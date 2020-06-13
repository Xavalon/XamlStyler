// (c) Xavalon. All rights reserved.

using Irony.Parsing;
using System;

namespace Xavalon.XamlStyler.MarkupExtensions.Parser
{
    public class PositionalArgument : Argument
    {
        public Value Value { get; }

        public PositionalArgument(Value value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        public static PositionalArgument Create(ParseTreeNode node)
        {
            var value = Value.Create(node);

            if (value == null)
            {
                return null;
            }

            return new PositionalArgument(value);
        }
    }
}