using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using Xavalon.XamlStyler.Mac.Services.XamlFormatting;
using Xavalon.XamlStyler.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Mac.CommandHandlers
{
    public class FormatXamlCommandHandler : CommandHandler
    {
        private IXamlFormattingService XamlFormattingService => Container.Instance.Resolve<IXamlFormattingService>();
        private IXamlStylerOptionsService XamlStylerOptionsService => Container.Instance.Resolve<IXamlStylerOptionsService>();

        protected override void Run()
        {
            var document = IdeApp.Workbench.ActiveDocument;
            var stylerOptions = XamlStylerOptionsService.GetDocumentOptions(document);
            XamlFormattingService.TryFormatXamlDocument(document, stylerOptions);
        }

        protected override void Update(CommandInfo info)
        {
            var document = IdeApp.Workbench.ActiveDocument;
            var isDocumentFormattable = XamlFormattingService.IsDocumentFormattable(document);
            info.Enabled = isDocumentFormattable;
            info.Visible = isDocumentFormattable;
        }
    }
}