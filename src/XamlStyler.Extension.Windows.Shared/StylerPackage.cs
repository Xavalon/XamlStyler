// © Xavalon. All rights reserved.

using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Extension.Windows.Extensions;
using Xavalon.XamlStyler.Extension.Windows.Helpers;

using Task = System.Threading.Tasks.Task;

namespace Xavalon.XamlStyler.Extension.Windows
{
    [ProvideLoadKey("Standard", "2.1", "XAML Styler", "Xavalon", 104)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#1110", "#1112", "1.0", IconResourceID = 1400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Guids.GuidXamlStylerPackageString)]
    [ProvideService(typeof(StylerService), IsAsyncQueryable = true)]
    [ProvideOptionPage(typeof(PackageOptions), "XAML Styler", "General", 101, 106, true)]
    [ProvideProfile(typeof(PackageOptions), "XAML Styler", "XAML Styler Settings", 106, 107, true, DescriptionResourceID = 108)]
    [ProvideAutoLoad(Guids.UIContextGuidString, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideUIContextRule(Guids.UIContextGuidString, name: "XAML load", expression: "Dotxaml", termNames: new[] { "Dotxaml" }, termValues: new[] { "HierSingleSelectionName:.xaml$" })]
    public sealed partial class StylerPackage : AsyncPackage
    {
        private IVsUIShell uiShell;
        private OleMenuCommandService menuCommandService;
        private CommandEvents saveCommandEvents;
        private CommandEvents saveAllCommandEvents;
        private OptionsHelper optionsHelper;

        public DTE IDE { get; private set; }
        public DTE2 IDE2 => (DTE2)this.IDE;

        public StylerPackage()
        {
            Trace.WriteLine($"Entering constructor for: {this.ToString()}");
        }

        // https://github.com/Microsoft/vs-threading/blob/master/doc/cookbook_vs.md#should-i-await-a-task-with-configureawaitfalse
        [SuppressMessage("Reliability", "CA2007:Do not directly await a Task", Justification = "Not recommended in VS-specific code")]
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            Trace.WriteLine($"Entering Initialize() of: {this.ToString()}");
            base.Initialize();

            this.IDE = await this.GetServiceAsync(typeof(DTE)) as DTE;
            Assumes.Present(this.IDE);

            this.uiShell = await this.GetServiceAsync(typeof(IVsUIShell)) as IVsUIShell;
            Assumes.Present(this.uiShell);

            this.menuCommandService = await this.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(this.menuCommandService);

            this.optionsHelper = new OptionsHelper(this);

            this.AddCommandHandlers();
        }

        private void AddCommandHandlers()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Format XAML File Command
            this.menuCommandService.AddCommand(new MenuCommand(
                (s, e) => this.FormatFileEventHandler(),
                new CommandID(Guids.GuidXamlStylerMenuSet, (int)Constants.CommandIDFormatXamlFile)));

            // Format All XAML Command
            this.menuCommandService.AddCommand(new MenuCommand(
                (s, e) => this.FormatSolutionEventHandler(),
                new CommandID(Guids.GuidXamlStylerMenuSet, (int)Constants.CommandIDFormatAllXaml)));

            // Format Selected XAML Command
            this.menuCommandService.AddCommand(new MenuCommand(
                (s, e) => this.FormatSelectedFilesEventHandler(),
                new CommandID(Guids.GuidXamlStylerMenuSet, (int)Constants.CommandIDFormatSelectedXaml)));

            // File.Save Command
            this.saveCommandEvents = this.IDE2.Events.CommandEvents[$"{{{Guids.GuidVsStd97CmdIDString}}}", 331];
            this.saveCommandEvents.BeforeExecute += this.OnFileSave;

            // File.SaveAll Command
            this.saveAllCommandEvents = this.IDE2.Events.CommandEvents[$"{{{Guids.GuidVsStd97CmdIDString}}}", 224];
            this.saveAllCommandEvents.BeforeExecute += this.OnFileSaveAll;
        }

        private void FormatFileEventHandler()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.uiShell.SetWaitCursor();
            if (TryGetActiveDocument(out Document document))
            {
                this.FormatDocument(document);
            }
        }

        private void FormatSolutionEventHandler()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.uiShell.SetWaitCursor();

            IEnumerable<ProjectItem> projectItems = ProjectItemHelper.GetAllProjectItems(this.IDE2.Solution)
                .Where(_ => _.IsXaml() && _.IsFormatable());

            this.FormatDocuments(projectItems);
        }

        private void FormatSelectedFilesEventHandler()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.uiShell.SetWaitCursor();

            IEnumerable<ProjectItem> projectItems = ProjectItemHelper.GetSelectedProjectItemsRecursively(this)
                .Where(_ => _.IsXaml() && _.IsFormatable());

            this.FormatDocuments(projectItems);
        }

        private void OnFileSave(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IStylerOptions globalOptions = this.optionsHelper.GetGlobalStylerOptions();
            if (!globalOptions.FormatOnSave)
            {
                return;
            }

            if (TryGetActiveDocument(out Document document))
            {
                IStylerOptions options = this.optionsHelper.GetDocumentStylerOptions(document);
                if (options.FormatOnSave)
                {
                    this.FormatDocument(document, options);
                }
            }
        }

        private void OnFileSaveAll(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IStylerOptions globalOptions = this.optionsHelper.GetGlobalStylerOptions();
            if (!globalOptions.FormatOnSave)
            {
                return;
            }

            // Use parallel processing, but only on the documents that are formatable (to avoid the overhead of Task creating when it's not necessary).
            var jobs = new List<Func<Action>>();
            foreach (Document document in this.IDE2.Documents)
            {
                // Skip unopened documents.
                if (document.ActiveWindow != null)
                {
                    continue;
                }

                var xamlLanguageOptions = document.GetXamlLanguageOptions();
                if (xamlLanguageOptions.IsFormatable)
                {
                    IStylerOptions options = this.optionsHelper.GetDocumentStylerOptions(document);
                    if (options.FormatOnSave)
                    {
                        jobs.Add(SetupFormatDocumentContinuation(document, options, xamlLanguageOptions));
                    }
                }
            }

            // Executes job() in parallel, then execute finish() sequentially.
            foreach (Action finish in jobs.AsParallel().Select(job => job()))
            {
                finish();
            }
        }

        private bool TryGetActiveDocument(out Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                document = this.IDE2.ActiveDocument;
                return document != null;
            }
            catch
            {
                // EnvDTE80.DTE2.get_ActiveDocument() *may* throw an ArgumentNullException for some documents.
            }

            document = null;
            return false;
        }

        private void ShowMessageBox(Exception ex)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Guid clsid = Guid.Empty;
            this.uiShell.ShowMessageBox(
                0,
                ref clsid,
                $"Error in {this.GetType().Name}:",
                $"{ex.Message}\r\n\r\nIf this deems a malfunctioning of styler, please kindly submit an issue at https://github.com/Xavalon/XamlStyler.",
                String.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0, // false
                out _);
        }
    }
}