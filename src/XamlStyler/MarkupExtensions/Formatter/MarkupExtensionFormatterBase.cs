// (c) Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xavalon.XamlStyler.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.MarkupExtensions.Formatter
{
    internal abstract class MarkupExtensionFormatterBase
    {
        protected readonly MarkupExtensionFormatter markupExtensionFormatter;

        protected MarkupExtensionFormatterBase(MarkupExtensionFormatter markupExtensionFormatter)
        {
            this.markupExtensionFormatter = markupExtensionFormatter;
        }

        public IEnumerable<string> FormatArguments(MarkupExtension markupExtension, bool isNested = false)
        {
            return markupExtension.Arguments.Any()
                ? MarkupExtensionFormatterBase.Format($"{{{markupExtension.TypeName} ", this.FormatArguments(markupExtension.Arguments, isNested: isNested), "}")
                : new string[] { $"{{{markupExtension.TypeName}}}" };
        }

        protected abstract IEnumerable<string> FormatArguments(Collection<Argument> arguments, bool isNested = false);

        protected IEnumerable<string> FormatArgument(Argument argument, bool isNested = false)
        {
            var type = argument.GetType();

            if (type == typeof(NamedArgument))
            {
                return this.FormatNamedArgument((NamedArgument)argument);
            }

            if (type == typeof(PositionalArgument))
            {
                var positionalArgument = (PositionalArgument)argument;

                if (positionalArgument.Value.GetType() == typeof(MarkupExtension))
                {
                    return this.markupExtensionFormatter.Format((MarkupExtension)positionalArgument.Value, isNested: isNested);
                }
                else
                {
                    return this.FormatPositionalArgument(positionalArgument);
                }
            }

            throw new ArgumentException($"Unhandled type {type.FullName}", nameof(argument));
        }

        private static IEnumerable<string> Format(string prefix, IEnumerable<string> lines, string suffix = null)
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

        private IEnumerable<string> FormatNamedArgument(NamedArgument namedArgument)
        {
            return MarkupExtensionFormatterBase.Format($"{namedArgument.Name}=", this.FormatValue(namedArgument.Value));
        }

        private IEnumerable<string> FormatPositionalArgument(PositionalArgument positionalArgument)
        {
            return this.FormatValue(positionalArgument.Value);
        }

        private static IEnumerable<string> FormatLiteralValue(LiteralValue literalValue)
        {
            return new[]
            {
                literalValue.Value
            };
        }

        private IEnumerable<string> FormatValue(Value value)
        {
            var type = value.GetType();

            if (type == typeof(LiteralValue))
            {
                return MarkupExtensionFormatterBase.FormatLiteralValue((LiteralValue)value);
            }

            if (type == typeof(MarkupExtension))
            {
                return this.FormatArguments((MarkupExtension)value);
            }

            throw new ArgumentException($"Unhandled type {type.FullName}", nameof(value));
        }
    }
}