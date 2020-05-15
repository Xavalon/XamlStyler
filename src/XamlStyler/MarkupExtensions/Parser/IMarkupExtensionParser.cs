// (c) Xavalon. All rights reserved.

namespace Xavalon.XamlStyler.MarkupExtensions.Parser
{
    public interface IMarkupExtensionParser
    {
        bool TryParse(string sourceText, out MarkupExtension graph);
    }
}