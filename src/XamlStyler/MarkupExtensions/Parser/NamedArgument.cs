// (c) Xavalon. All rights reserved.

using Irony.Parsing;
using System;
using System.Linq;

namespace Xavalon.XamlStyler.MarkupExtensions.Parser
{
    public class NamedArgument : Argument
    {
        public string Name { get; }

        public Value Value { get; }

        public NamedArgument(string name, Value value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Name = name;
            this.Value = value;
        }

        public static NamedArgument Create(ParseTreeNode node)
        {
            if (node.Term.Name != XamlMarkupExtensionGrammar.NamedArgumentTerm)
            {
                return null;
            }

            if (node.ChildNodes.Count != 2)
            {
                throw new Exception("Named argument count mismatch");
            }

            return new NamedArgument(GetMemberName(node.ChildNodes.First()), Value.Create(node.ChildNodes.Last()));
        }

        private static string GetMemberName(ParseTreeNode node)
        {
            if (node.Term.Name != XamlMarkupExtensionGrammar.MemberNameTerm)
            {
                throw new Exception("Unparsable extension");
            }

            return node.Token.Text;
        }
    }
}