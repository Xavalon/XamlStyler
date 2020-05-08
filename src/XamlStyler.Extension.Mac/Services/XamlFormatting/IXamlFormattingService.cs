// © Xavalon. All rights reserved.

using MonoDevelop.Ide.Gui;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Mac.Services.XamlFormatting
{
    public interface IXamlFormattingService
    {
        bool TryFormatXamlDocument(Document document, IStylerOptions stylerOptions);

        bool TryFormatXaml(ref string xamlText, IStylerOptions stylerOptions);

        bool IsDocumentFormattable(Document document);
    }
}