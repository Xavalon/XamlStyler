namespace XamlStyler.Core.Parser
{
    public enum MarkupExtensionParsingModeEnum
    {
        // ReSharper disable InconsistentNaming
        START,
        MARKUP_NAME,
        NAME_VALUE_PAIR,
        MARKUP_EXTENSION_VALUE,
        QUOTED_LITERAL_VALUE,
        LITERAL_VALUE,
        END,
        UNEXPECTED
        // ReSharper restore InconsistentNaming
    }
}