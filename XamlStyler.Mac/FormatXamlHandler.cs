using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using Xavalon.XamlStyler.Core;
using MonoDevelop.Components;

namespace Xavalon.XamlStyler.Mac
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
            if (doc?.FileName.Extension.ToLowerInvariant() == ".xaml")
            {
                info.Enabled = true;
                info.Visible = true;
            }
            else
            {
                info.Visible = false;
            }
        }
    }
}