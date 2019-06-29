using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System.Linq;
using Xavalon.XamlStyler.Core;

namespace Xavalon.XamlStyler.Mac
{
    public class FormatXamlBatchHandler : CommandHandler
    {
        protected override void Run()
        {
            var item = IdeApp.ProjectOperations.CurrentSelectedItem;
            if (item is Solution sln)
            {
                BatchProcessSolution(sln);
                return;
            }

            if (item is Project prj)
            {
                BatchProcessProject(prj);
            }
        }

        private void BatchProcessSolution(Solution sln)
        {
            foreach (var prj in sln.GetAllProjects())
            {
                BatchProcessProject(prj);
            }
        }

        private void BatchProcessProject(Project prj)
        {
            LoggingService.LogDebug($"Processing {prj.Name} project...");
            foreach (var file in prj.Files.Where(f => f.Name.EndsWith(".xaml", System.StringComparison.OrdinalIgnoreCase)).ToArray())
            {
                ProcessFileInPlace(file);
            }
        }

        private void ProcessFileInPlace(ProjectFile file)
        {
            var options = StylerOptionsConfiguration.GetOptionsForDocument(file.Name, file.Project);
            var styler = new StylerService(options);

            LoggingService.LogDebug($"Processing {file.FilePath} in-place");
            var content = System.IO.File.ReadAllText(file.FilePath);

            var styledXaml = styler.StyleDocument(content);

            System.IO.File.WriteAllText(file.FilePath, styledXaml);

            var openedFile = IdeApp.Workbench.Documents.FirstOrDefault(f => f.FileName.FullPath == file.FilePath);
            if (openedFile != null)
            {
                LoggingService.LogDebug($"Reloading {file.FilePath} in editor window");
                openedFile.Reload();
            }
        }
    }
}