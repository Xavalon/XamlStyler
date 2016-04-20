namespace XamlStyler.Core.MarkupExtensions.Parser
{
    public interface IMarkupExtensionParser
    {
        bool TryParse(string sourceText, out MarkupExtension graph);
    }
}