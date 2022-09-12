// (c) Xavalon. All rights reserved.

using Microsoft.VisualStudio.Text;
using MonoDevelop.Ide.Gui;
using System;
using Xavalon.XamlStyler.Extension.Mac.Extensions;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Extension.Mac.Services.XamlFormatting
{
    public class XamlFormattingService : IXamlFormattingService
    {
        public bool TryFormatXamlDocument(Document document, IStylerOptions stylerOptions)
        {
            var textBuffer = document.TextBuffer;
            var currentTextSnapshot = textBuffer.CurrentSnapshot;
            var xamlText = currentTextSnapshot.GetText();
            if (!TryFormatXaml(ref xamlText, stylerOptions, document.GetXamlLanguageOptions()))
            {
                return false;
            }

            var replaceSpan = new Span(0, currentTextSnapshot.Length);
            textBuffer.Replace(replaceSpan, xamlText);

            document.IsDirty = true;
            return true;
        }

        public bool TryFormatXaml(ref string xamlText, IStylerOptions stylerOptions, XamlLanguageOptions xamlLanguageOptions)
        {
            var stylerService = new StylerService(stylerOptions, xamlLanguageOptions);
            var styledText = stylerService.StyleDocument(xamlText);
            if (xamlText == styledText)
            {
                return false;
            }

            xamlText = styledText;
            return true;
        }

        public bool IsDocumentFormattable(Document document)
        {
            if (document is null || document.IsViewOnly)
            {
                return false;
            }

            var isXamlFile = string.Equals(document.FileName.Extension, Constants.XamlFileExtension, StringComparison.InvariantCultureIgnoreCase);
            return isXamlFile;
        }
    }
}