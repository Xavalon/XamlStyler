// © Xavalon. All rights reserved.

namespace Xavalon.XamlStyler.Core.MarkupExtensions.Parser
{
    public interface IMarkupExtensionParser
    {
        bool TryParse(string sourceText, out MarkupExtension graph);
    }
}