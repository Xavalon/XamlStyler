// © Xavalon. All rights reserved.

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Extension.Windows.Extensions;

namespace Xavalon.XamlStyler.Extension.Windows.Helpers
{
    public sealed class OptionsHelper
    {
        private readonly StylerPackage package;

        public OptionsHelper(StylerPackage package)
        {
            this.package = package;
        }

        public IStylerOptions GetGlobalStylerOptions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return this.package.GetDialogPage(typeof(PackageOptions)).AutomationObject as IStylerOptions;
        }

        public IStylerOptions GetDocumentStylerOptions(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Properties xamlEditorProps = this.package.IDE.Properties["TextEditor", "XAML"];
            IStylerOptions stylerOptions = this.GetGlobalStylerOptions();

            string solutionPath = String.IsNullOrEmpty(this.package.IDE.Solution?.FullName)
                ? String.Empty
                : (stylerOptions.SearchToDriveRoot ? Path.GetPathRoot(this.package.IDE.Solution.FullName) : Path.GetDirectoryName(this.package.IDE.Solution.FullName));

            Project project = document.ProjectItem?.ContainingProject;
            string configPath = GetConfigPathForItem(document.Path, solutionPath, project);

            if (configPath != null)
            {
                stylerOptions = ((StylerOptions)stylerOptions).Clone();
                stylerOptions.ConfigPath = configPath;
            }

            if (stylerOptions.UseVisualStudioIndentSize)
            {
                if (Int32.TryParse(xamlEditorProps.Item("IndentSize").Value.ToString(), out int outIndentSize)
                    && (outIndentSize > 0))
                {
                    stylerOptions.IndentSize = outIndentSize;
                }
            }

            if (stylerOptions.UseVisualStudioIndentWithTabs)
            {
                stylerOptions.IndentWithTabs = (bool)xamlEditorProps.Item("InsertTabs").Value;
            }

            return stylerOptions;
        }

        private static string GetConfigPathForItem(string path, string solutionRoot, Project project)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            string projectFullName = project?.FullName;
            string projectDirectory = projectFullName.IsNullOrEmpty()
                ? String.Empty
                : Path.GetDirectoryName(projectFullName);

            IEnumerable<string> configPaths = path.StartsWith(solutionRoot, StringComparison.InvariantCultureIgnoreCase)
                ? OptionsHelper.GetConfigPathBetweenPaths(path, solutionRoot)
                : OptionsHelper.GetConfigPathBetweenPaths(path, projectDirectory);

            // Find the FullPath of "Settings.XamlStyler" ref in project.
            IEnumerable<string> filePathsInProject = project?.ProjectItems.Cast<ProjectItem>()
                .Where(_ => { ThreadHelper.ThrowIfNotOnUIThread(); return String.Equals(_.Name, "Settings.XamlStyler", StringComparison.Ordinal); })
                .SelectMany(_ => { ThreadHelper.ThrowIfNotOnUIThread(); return _.Properties.Cast<Property>(); })
                .Where(_ => { ThreadHelper.ThrowIfNotOnUIThread(); return String.Equals(_.Name, "FullPath", StringComparison.Ordinal); })
                .Select(_ => { ThreadHelper.ThrowIfNotOnUIThread(); return _.Value as string; });

            if (filePathsInProject != null)
            {
                configPaths = configPaths.Concat(filePathsInProject);
            }

            return configPaths.FirstOrDefault(File.Exists);
        }

        // Searches for configuration file up through solution root directory.
        private static IEnumerable<string> GetConfigPathBetweenPaths(string path, string root)
        {
            string configDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory)
                ? path
                : Path.GetDirectoryName(path);

            while (configDirectory?.StartsWith(root, StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                yield return Path.Combine(configDirectory, "Settings.XamlStyler");
                // If the root directory is given as an argument, the result will be null.
                configDirectory = Path.GetDirectoryName(configDirectory);
            }
        }
    }
}
