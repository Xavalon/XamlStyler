using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using Xavalon.XamlStyler.Mac.Services.XamlFormatting;
using Xavalon.XamlStyler.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Mac.CommandHandlers
{
    public class FormatXamlCommandHandler : CommandHandler
    {
        private readonly IXamlFormattingService _xamlFormattingService;
        private readonly IXamlStylerOptionsService _xamlStylerOptionsService;

        public FormatXamlCommandHandler()
        {
            _xamlFormattingService = Container.Instance.Resolve<IXamlFormattingService>();
            _xamlStylerOptionsService = Container.Instance.Resolve<IXamlStylerOptionsService>();
        }

        protected override void Run()
        {
            var document = IdeApp.Workbench.ActiveDocument;
            var stylerOptions = _xamlStylerOptionsService.GetDocumentOptions(document);
            _xamlFormattingService.FormatXamlDocument(document, stylerOptions);
        }

        protected override void Update(CommandInfo info)
        {
            var document = IdeApp.Workbench.ActiveDocument;
            var isDocumentFormattable = _xamlFormattingService.IsDocumentFormattable(document);
            info.Enabled = isDocumentFormattable;
            info.Visible = isDocumentFormattable;
        }
    }
}