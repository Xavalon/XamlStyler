// © Xavalon. All rights reserved.

using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xavalon.XamlStyler.Core.MarkupExtensions.Parser
{
    public class MarkupExtension : Value
    {
        public string TypeName { get; }

        public Argument[] Arguments { get; }

        public MarkupExtension(string typeName, params Argument[] arguments)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            this.TypeName = typeName;
            this.Arguments = arguments;
        }

        public static new MarkupExtension Create(ParseTreeNode node)
        {
            if (node.Term.Name != XamlMarkupExtensionGrammar.MarkupExtensionTerm)
            {
                return null;
            }

            return new MarkupExtension(
                GetTypeName(node.ChildNodes.First()),
                GetArguments(node.ChildNodes.Skip(1)).ToArray());
        }

        private static string GetTypeName(ParseTreeNode node)
        {
            if (node.Term.Name != XamlMarkupExtensionGrammar.TypeNameTerm)
            {
                return null;
            }

            return node.Token.Text;
        }

        private static IEnumerable<Argument> GetArguments(IEnumerable<ParseTreeNode> nodes)
        {
            foreach (var node in nodes)
            {
                var argument = PositionalArgument.Create(node) ?? (Argument)NamedArgument.Create(node);

                if (argument != null)
                {
                    yield return argument;
                }
                else
                {
                    // Unwrap argument.
                    foreach (var markupExtensionArgument in MarkupExtension.GetArguments(node.ChildNodes))
                    {
                        yield return markupExtensionArgument;
                    }
                }
            }
        }
    }
}