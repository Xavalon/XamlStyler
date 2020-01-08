using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xavalon.XamlStyler.Mac.Services.XamlFiles;
using Xavalon.XamlStyler.Mac.Services.XamlFormatting;
using Xavalon.XamlStyler.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Mac.CommandHandlers
{
    public class BatchFormatXamlCommandHandler : CommandHandler
    {
        private readonly IXamlFilesService _xamlFilesService;
        private readonly IXamlStylerOptionsService _xamlStylerOptionsService;
        private readonly IXamlFormattingService _xamlFormattingService;

        public BatchFormatXamlCommandHandler()
        {
            _xamlFilesService = Container.Instance.Resolve<IXamlFilesService>();
            _xamlStylerOptionsService = Container.Instance.Resolve<IXamlStylerOptionsService>();
            _xamlFormattingService = Container.Instance.Resolve<IXamlFormattingService>();
        }

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
                    var solutionFiles = _xamlFilesService.FindAllXamlFilePaths(solution);
                    return solutionFiles;
                case Project project:
                    var projectFiles = _xamlFilesService.FindAllXamlFilePaths(project);
                    return projectFiles;
                default:
                    LoggingService.LogDebug($"Unknown selected item: {selectedItem.GetType().FullName}");
                    return new List<string>();
            }
        }

        private void ProcessXamlFile(string xamlFilePath, Solution solution)
        {
            var stylerOptions = _xamlStylerOptionsService.GetDocumentOptions(xamlFilePath, solution);
            var xamlFileText = File.ReadAllText(xamlFilePath); ;
            if (!_xamlFormattingService.TryFormatXaml(ref xamlFileText, stylerOptions))
            {
                return;
            }

            File.WriteAllText(xamlFilePath, xamlFileText);
            if (IsFileCurrentlyOpened(xamlFilePath, out var openedDocument))
            {
                openedDocument.Reload();
            }
        }

        private bool IsFileCurrentlyOpened(string filePath, out Document openedDocument)
        {
            openedDocument = IdeApp.Workbench
                                   .Documents
                                   .FirstOrDefault(document => document.FilePath.FileName == filePath);

            return openedDocument != null;
        }
    }
}