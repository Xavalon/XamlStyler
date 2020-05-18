// (c) Xavalon. All rights reserved.

using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xavalon.XamlStyler.Extension.Mac.Services.XamlFiles;
using Xavalon.XamlStyler.Extension.Mac.Services.XamlFormatting;
using Xavalon.XamlStyler.Extension.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Extension.Mac.CommandHandlers
{
    public class BatchFormatXamlCommandHandler : CommandHandler
    {
        private IXamlFilesService XamlFilesService => Container.Instance.Resolve<IXamlFilesService>();
        private IXamlStylerOptionsService XamlStylerOptionsService => Container.Instance.Resolve<IXamlStylerOptionsService>();
        private IXamlFormattingService XamlFormattingService => Container.Instance.Resolve<IXamlFormattingService>();

        protected override void Run()
        {
            var selectedItem = IdeApp.ProjectOperations.CurrentSelectedItem;
            var selectedSolution = IdeApp.ProjectOperations.CurrentSelectedSolution;
            var xamlFilePaths = GetXamlFilePaths(selectedItem);
            foreach (var xamlFilePath in xamlFilePaths)
            {
                ProcessXamlFile(xamlFilePath, selectedSolution);
            }
        }

        private List<string> GetXamlFilePaths(object selectedItem)
        {
            switch (selectedItem)
            {
                case Solution solution:
                    var solutionFiles = XamlFilesService.FindAllXamlFilePaths(solution);
                    return solutionFiles;
                case Project project:
                    var projectFiles = XamlFilesService.FindAllXamlFilePaths(project);
                    return projectFiles;
                default:
                    LoggingService.LogDebug($"Unknown selected item: {selectedItem.GetType().FullName}");
                    return new List<string>();
            }
        }

        private void ProcessXamlFile(string xamlFilePath, Solution solution)
        {
            var stylerOptions = XamlStylerOptionsService.GetDocumentOptions(xamlFilePath, solution);
            if (IsFileCurrentlyOpened(xamlFilePath, out var openedDocument))
            {
                XamlFormattingService.TryFormatXamlDocument(openedDocument, stylerOptions);
                return;
            }

            var xamlFileText = File.ReadAllText(xamlFilePath);
            if (!XamlFormattingService.TryFormatXaml(ref xamlFileText, stylerOptions))
            {
                return;
            }

            File.WriteAllText(xamlFilePath, xamlFileText);
        }

        private bool IsFileCurrentlyOpened(string filePath, out Document openedDocument)
        {
            openedDocument = IdeApp.Workbench
                                   .Documents
                                   .FirstOrDefault(document => document.FilePath.ToString() == filePath);

            return openedDocument != null && openedDocument.TextBuffer != null;
        }
    }
}