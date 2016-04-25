using System;
using Irony.Parsing;

namespace XamlStyler.Core.MarkupExtensions.Parser
{
    public class LiteralValue : Value
    {
        public string Value { get; }

        public LiteralValue(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public new static LiteralValue Create(ParseTreeNode node)
        {
            if (node.Term.Name != XamlMarkupExtensionGrammar.StringTerm)
                return null;

            return new LiteralValue(node.Token.Text);
        }

    }
}