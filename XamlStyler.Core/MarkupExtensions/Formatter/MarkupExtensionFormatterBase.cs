// © Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.Core.MarkupExtensions.Formatter
{
    internal abstract class MarkupExtensionFormatterBase
    {
        protected readonly MarkupExtensionFormatter markupExtensionFormatter;

        protected MarkupExtensionFormatterBase(MarkupExtensionFormatter markupExtensionFormatter)
        {
            this.markupExtensionFormatter = markupExtensionFormatter;
        }

        public IEnumerable<string> Format(MarkupExtension markupExtension)
        {
            return markupExtension.Arguments.Any()
                ? this.Format($"{{{markupExtension.TypeName} ", this.Format(markupExtension.Arguments), "}")
                : new string[] { $"{{{markupExtension.TypeName}}}" };
        }

        protected abstract IEnumerable<string> Format(Argument[] arguments);

        protected IEnumerable<string> Format(Argument argument)
        {
            var type = argument.GetType();

            if (type == typeof(NamedArgument))
            {
                return this.Format((NamedArgument)argument);
            }

            if (type == typeof(PositionalArgument))
            {
                var positionalArgument = (PositionalArgument)argument;

                if (positionalArgument.Value.GetType() == typeof(MarkupExtension))
                {
                    return this.markupExtensionFormatter.Format((MarkupExtension)positionalArgument.Value);
                }
                else
                {
                    return this.Format(positionalArgument);
                }
            }

            throw new ArgumentException($"Unhandled type {type.FullName}", nameof(argument));
        }

        private IEnumerable<string> Format(string prefix, IEnumerable<string> lines, string suffix = null)
        {
            var list = new List<string>();

            string queued = prefix + lines.First();
            foreach (var line in lines.Skip(1))
            {
                list.Add(queued);
                queued = new String(' ', prefix.Length) + line;
            }

            list.Add(queued + suffix);

            return list;
        }

        private IEnumerable<string> Format(NamedArgument namedArgument)
        {
            return this.Format($"{namedArgument.Name}=", this.Format(namedArgument.Value));
        }

        private IEnumerable<string> Format(PositionalArgument positionalArgument)
        {
            return this.Format(positionalArgument.Value);
        }

        private IEnumerable<string> Format(LiteralValue literalValue)
        {
            return new[]
            {
                literalValue.Value
            };
        }

        private IEnumerable<string> Format(Value value)
        {
            var type = value.GetType();

            if (type == typeof(LiteralValue))
            {
                return this.Format((LiteralValue)value);
            }

            if (type == typeof(MarkupExtension))
            {
                return this.Format((MarkupExtension)value);
            }

            throw new ArgumentException($"Unhandled type {type.FullName}", nameof(value));
        }
    }
}