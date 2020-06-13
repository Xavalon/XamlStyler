// (c) Xavalon. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xavalon.XamlStyler.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.MarkupExtensions.Formatter
{
    internal class SingleLineMarkupExtensionFormatter : MarkupExtensionFormatterBase
    {
        internal SingleLineMarkupExtensionFormatter(MarkupExtensionFormatter markupExtensionFormatter)
            : base(markupExtensionFormatter)
        {
        }

        protected override IEnumerable<string> FormatArguments(Collection<Argument> arguments, bool isNested = false)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var argument in arguments)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(", ");
                }

                foreach (var line in this.FormatArgument(argument, isNested: true))
                {
                    stringBuilder.Append(line);
                }
            }

            yield return stringBuilder.ToString();
        }
    }
}