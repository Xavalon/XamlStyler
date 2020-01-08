using MonoDevelop.Ide.Gui;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Mac.Services.XamlFormatting
{
    public interface IXamlFormattingService
    {
        void FormatXamlDocument(Document document, IStylerOptions stylerOptions);

        bool TryFormatXaml(ref string xamlText, IStylerOptions stylerOptions);

        bool IsDocumentFormattable(Document document);
    }
}