using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using Xavalon.XamlStyler.Core;

namespace Xavalon.XamlStyler.XamarinStudio
{
	public class FormatXamlHandler : CommandHandler
	{
		protected override void Run()
		{
			var options = StylerOptionsConfiguration.ReadFromUserProfile();
			var styler = new StylerService(options);

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

		protected override void Update(CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			if (doc != null || doc.FileName.Extension.ToLowerInvariant() == ".xaml")
			{
				LoggingService.LogInfo($"XamlStyler: Filename is {doc.FileName}, extension is ENABLED");
				info.Enabled = info.Visible = true;
			}
			else
			{
				LoggingService.LogInfo($"XamlStyler: Filename is {doc.FileName}, extension is DISABLED");
				info.Visible = false;
			}
		}
	}
}