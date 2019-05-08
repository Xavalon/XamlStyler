using System.Linq;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using Xavalon.XamlStyler.Core;

namespace Xavalon.XamlStyler.VisualStudioForMac
{
    public class FormatXamlBatchHandler : CommandHandler
    {
        protected override void Run()
        {
            var options = StylerOptionsConfiguration.ReadFromUserProfile();
            var styler = new StylerService(options);
            
            var item = IdeApp.ProjectOperations.CurrentSelectedItem;

            if (item is Solution sln)
            {
                BatchProcessSolution(sln, styler);
                return;
            }

            if (item is Project prj)
            {
                BatchProcessProject(prj, styler);
            }
        }

        private void BatchProcessSolution(Solution sln, StylerService styler)
        {
            foreach (var prj in sln.GetAllProjects())
            {
                BatchProcessProject(prj, styler);
            }
        }

        private void BatchProcessProject(Project prj, StylerService styler)
        {
            LoggingService.LogDebug($"Processing {prj.Name} project...");
            foreach (var file in prj.Files.Where(f => f.Name.EndsWith(".xaml", System.StringComparison.OrdinalIgnoreCase)).ToArray())
            {
                ProcessFileInPlace(file, styler);
            }
        }

        private void ProcessFileInPlace(ProjectFile file, StylerService styler)
        {
            LoggingService.LogDebug($"Processing {file.FilePath} in-place");
            var content = System.IO.File.ReadAllText(file.FilePath);

            var styledXaml = styler.StyleDocument(content);

            System.IO.File.WriteAllText(file.FilePath, styledXaml);

            var openedFile =
                IdeApp.Workbench.Documents.FirstOrDefault(f => f.FileName.FullPath == file.FilePath);

            if (openedFile != null)
            {
                LoggingService.LogDebug($"Reloading {file.FilePath} in editor window");
                openedFile.Reload();
            }
        }
    }
}