using System.Linq;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.XamarinStudio
{
	public class FormatXamlBatchHandler : CommandHandler
	{
		protected override void Run()
		{
			var options = StylerOptionsConfiguration.ReadFromUserProfile();
			var styler = new StylerService(options);

			var item = IdeApp.ProjectOperations.CurrentSelectedItem;

			var sln = item as Solution;
			if (sln != null)
			{
				BatchProcessSolution(sln, options, styler);
				return;
			}

			var prj = item as Project;
			if (prj != null)
			{
				BatchProcessProject(prj, options, styler);
			}
		}

		private void BatchProcessSolution(Solution sln, StylerOptions options, StylerService styler)
		{
			foreach (var prj in sln.GetAllProjects())
			{
				BatchProcessProject(prj, options, styler);
			}
		}

		private void BatchProcessProject(Project prj, StylerOptions options, StylerService styler)
		{
			LoggingService.LogDebug($"Processing {prj.Name} project...");
			foreach (var file in prj.Files.Where(f => f.Name.EndsWith(".xaml", System.StringComparison.OrdinalIgnoreCase)).ToArray())
			{
				if (options.BatchOpenFiles)
				{
					OpenAndProcessFile(file, styler);
				}
				else
				{
					ProcessFileInPlace(file, styler);
				}
			}
		}

		private void OpenAndProcessFile(ProjectFile file, StylerService styler)
		{
			LoggingService.LogDebug($"Opening and processing {file.FilePath}");
			IdeApp.OpenFiles(new[] { new FileOpenInformation(file.FilePath) });

			var doc = IdeApp.Workbench.ActiveDocument;
			var edit = doc.Editor;

			if (edit != null)
			{
				var styledXaml = styler.StyleDocument(edit.Text);

				using (edit.OpenUndoGroup())
				{
					edit.RemoveText(0, edit.Text.Length);
					edit.InsertText(0, styledXaml);
				}
				doc.IsDirty = true;
			}
		}

		private void ProcessFileInPlace(ProjectFile file, StylerService styler)
		{
			LoggingService.LogDebug($"Processing {file.FilePath} in-place");
			var content = System.IO.File.ReadAllText(file.FilePath);

			var styledXaml = styler.StyleDocument(content);

			System.IO.File.WriteAllText(file.FilePath, styledXaml);

			var openedFile =
				IdeApp.Workbench.Documents.Where(f => f.FileName.FullPath == file.FilePath).FirstOrDefault();

			if (openedFile != null)
			{
				LoggingService.LogDebug($"Reloading {file.FilePath} in editor window");
				openedFile.Reload();
			}
		}
	}
}