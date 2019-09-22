using Gdk;
using Gtk;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using Xavalon.XamlStyler.Core;
using Span = Microsoft.VisualStudio.Text.Span;

namespace Xavalon.XamlStyler.Mac
{
    public class FormatXamlHandler : CommandHandler
    {
        protected override void Run()
        {
            var document = IdeApp.Workbench.ActiveDocument;

            if (!StylerOptionsConfiguration.IsFormatableDocument(document))
            {
                return;
            }

            var stylerOptions = StylerOptionsConfiguration.GetOptionsForDocument(document.FileName, document.Owner as Project);
            var styler = new StylerService(stylerOptions);
            if (document.Editor is null)
            {
                var textBuffer = document.TextBuffer;
                var currentSnapshot = textBuffer.CurrentSnapshot;
                var rawText = currentSnapshot.GetText();
                var styledText = styler.StyleDocument(rawText);
                var replaceSpan = new Span(0, rawText.Length);
                textBuffer.Replace(replaceSpan, styledText);
            }
            else
            {
                var editor = document.Editor;
                using (editor.OpenUndoGroup())
                {
                    var styledText = styler.StyleDocument(editor.Text);
                    editor.Text = styledText;
                }
            }

            document.IsDirty = true;
        }

        protected override void Update(CommandInfo info)
        {
            var document = IdeApp.Workbench.ActiveDocument;

            var isDocumentFormattable = StylerOptionsConfiguration.IsFormatableDocument(document);
            info.Enabled = isDocumentFormattable;
            info.Visible = isDocumentFormattable;
        }
    }
}