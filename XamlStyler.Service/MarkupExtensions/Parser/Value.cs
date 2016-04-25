using Irony.Parsing;

namespace XamlStyler.Core.MarkupExtensions.Parser
{
    public abstract class Value
    {
        public static Value Create(ParseTreeNode node)
        {
            return LiteralValue.Create(node)
                   ?? (Value)MarkupExtension.Create(node);
        }

        public static implicit operator Value(string value)
        {
            return new LiteralValue(value);
        }
    }
}


