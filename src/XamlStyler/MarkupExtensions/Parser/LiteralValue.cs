// (c) Xavalon. All rights reserved.

using Irony.Parsing;
using System;

namespace Xavalon.XamlStyler.MarkupExtensions.Parser
{
    public class LiteralValue : Value
    {
        public string Value { get; }

        public LiteralValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Value = value;
        }

        public static new LiteralValue Create(ParseTreeNode node)
        {
            if (node.Term.Name != XamlMarkupExtensionGrammar.StringTerm)
            {
                return null;
            }

            return new LiteralValue(node.Token.Text);
        }
    }
}