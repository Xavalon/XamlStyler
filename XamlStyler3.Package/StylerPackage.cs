using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;
using Xavalon.XamlStyler.Package;

namespace Xavalon.XamlStyler3.Package
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#1110", "#1112", "1.0", IconResourceID = 1400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Guids.XamlStylerPackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideService(typeof(StylerService), IsAsyncQueryable = true)]
    [ProvideOptionPage(typeof(PackageOptions), "XAML Styler", "General", 101, 106, true)]
    [ProvideProfile(typeof(PackageOptions), "XAML Styler", "XAML Styler Settings", 106, 107, true, DescriptionResourceID = 108)]
    [ProvideAutoLoad(Guids.UIContextGuidString, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideUIContextRule(Guids.UIContextGuidString, name: "XAML load", expression: "Dotxaml", termNames: new[] { "Dotxaml" }, termValues: new[] { "HierSingleSelectionName:.xaml$" })]
    public sealed class StylerPackage : Microsoft.VisualStudio.Shell.Package
    {
        private DTE _dte;
        private Events _events;
        private CommandEvents _fileSaveAll;
        private CommandEvents _fileSaveSelectedItems;
        private IVsUIShell _uiShell;
        
        public StylerPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
        }
        
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));
            base.Initialize();

            _dte = GetService(typeof(DTE)) as DTE;

            if (_dte == null)
            {
                throw new NullReferenceException("DTE is null");
            }

            _uiShell = GetService(typeof(IVsUIShell)) as IVsUIShell;

            // Initialize command events listeners
            _events = _dte.Events;

            // File.SaveSelectedItems command
            _fileSaveSelectedItems = _events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 331];
            _fileSaveSelectedItems.BeforeExecute +=
                OnFileSaveSelectedItemsBeforeExecute;

            // File.SaveAll command
            _fileSaveAll = _events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 224];
            _fileSaveAll.BeforeExecute +=
                OnFileSaveAllBeforeExecute;

            //Initialize menu command
            // Add our command handlers for menu (commands must exist in the .vsct file)
            var menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (null != menuCommandService)
            {
                // Create the command for the menu item.
                var menuCommandId = new CommandID(Guids.CommandSetGuid, (int)PackageCommandIds.FormatXamlCommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
                menuCommandService.AddCommand(menuItem);
            }
        }

        private bool IsFormatableDocument(Document document)
        {
            bool isFormatableDocument;
            isFormatableDocument = !document.ReadOnly && document.Language == "XAML";

            if (!isFormatableDocument)
            {
                //xamarin
                isFormatableDocument = document.Language == "XML" && document.FullName.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase);
            }

            return isFormatableDocument;
        }

        private void OnFileSaveSelectedItemsBeforeExecute(string guid, int id, object customIn, object customOut,
                                                          ref bool cancelDefault)
        {
            Document document = _dte.ActiveDocument;

            if (IsFormatableDocument(document))
            {
                var options = GetDialogPage(typeof(PackageOptions)).AutomationObject as IStylerOptions;

                if (options.BeautifyOnSave)
                {
                    Execute(document);
                }
            }
        }

        private void OnFileSaveAllBeforeExecute(string guid, int id, object customIn, object customOut,
                                                ref bool cancelDefault)
        {
            // use parallel processing, but only on the documents that are formatable
            // (to avoid the overhead of Task creating when it's not necessary)

            List<Document> docs = new List<Document>();
            foreach (Document document in _dte.Documents)
            {
                if (IsFormatableDocument(document))
                {
                    docs.Add(document);
                }
            }

            Parallel.ForEach(docs, document =>
            {
                var options = GetDialogPage(typeof(PackageOptions)).AutomationObject as IStylerOptions;

                if (options.BeautifyOnSave)
                {
                    Execute(document);
                }
            }
                );
        }

        private void Execute(Document document)
        {
            if (!IsFormatableDocument(document))
            {
                return;
            }

            Properties xamlEditorProps = _dte.Properties["TextEditor", "XAML"];

            var stylerOptions = GetDialogPage(typeof(PackageOptions)).AutomationObject as IStylerOptions;

            var solutionPath = String.IsNullOrEmpty(_dte.Solution?.FullName)
                ? String.Empty
                : Path.GetDirectoryName(_dte.Solution.FullName);
            var configPath = GetConfigPathForItem(document.Path, solutionPath);

            if (configPath != null)
            {
                stylerOptions = ((StylerOptions)stylerOptions).Clone();
                stylerOptions.ConfigPath = configPath;
            }

            if (stylerOptions.UseVisualStudioIndentSize)
            {
                int outIndentSize;
                if (Int32.TryParse(xamlEditorProps.Item("IndentSize").Value.ToString(), out outIndentSize)
                    && (outIndentSize > 0))
                {
                    stylerOptions.IndentSize = outIndentSize;
                }
            }

            stylerOptions.IndentWithTabs = (bool)xamlEditorProps.Item("InsertTabs").Value;

            StylerService styler = new StylerService(stylerOptions);

            var textDocument = (TextDocument)document.Object("TextDocument");

            TextPoint currentPoint = textDocument.Selection.ActivePoint;
            int originalLine = currentPoint.Line;
            int originalOffset = currentPoint.LineCharOffset;

            EditPoint startPoint = textDocument.StartPoint.CreateEditPoint();
            EditPoint endPoint = textDocument.EndPoint.CreateEditPoint();

            string xamlSource = startPoint.GetText(endPoint);
            xamlSource = styler.StyleDocument(xamlSource);

            startPoint.ReplaceText(endPoint, xamlSource, 0);

            if (originalLine <= textDocument.EndPoint.Line)
            {
                textDocument.Selection.MoveToLineAndOffset(originalLine, originalOffset);
            }
            else
            {
                textDocument.Selection.GotoLine(textDocument.EndPoint.Line);
            }
        }

        private string GetConfigPathForItem(string path, string solutionPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return null;
                }

                while ((path = Path.GetDirectoryName(path)) != null && path.StartsWith(solutionPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    var configFile = Path.Combine(path, "Settings.XamlStyler");

                    if (File.Exists(configFile))
                    {
                        return configFile;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                _uiShell.SetWaitCursor();

                Document document = _dte.ActiveDocument;

                if (IsFormatableDocument(document))
                {
                    Execute(document);
                }
            }
            catch (Exception ex)
            {
                string title = string.Format("Error in {0}:", GetType().Name);
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}\r\n\r\nIf this deems a malfunctioning of styler, please kindly submit an issue at https://github.com/Xavalon/XamlStyler.",
                    ex.Message);

                ShowMessageBox(title, message);
            }
        }

        private void ShowMessageBox(string title, string message)
        {
            Guid clsid = Guid.Empty;
            int result;

            _uiShell.ShowMessageBox(
                0,
                ref clsid,
                title,
                message,
                String.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0, // false
                out result);
        }
    }
}
