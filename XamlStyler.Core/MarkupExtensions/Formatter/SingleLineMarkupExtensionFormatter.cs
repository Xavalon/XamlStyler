// © Xavalon. All rights reserved.

using System.Collections.Generic;
using System.Text;
using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.Core.MarkupExtensions.Formatter
{
    internal class SingleLineMarkupExtensionFormatter : MarkupExtensionFormatterBase
    {
        internal SingleLineMarkupExtensionFormatter(MarkupExtensionFormatter markupExtensionFormatter)
            : base(markupExtensionFormatter)
        {
        }

        protected override IEnumerable<string> Format(Argument[] arguments)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var argument in arguments)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(", ");
                }

                foreach (var line in this.Format(argument))
                {
                    stringBuilder.Append(line);
                }
            }

            yield return stringBuilder.ToString();
        }
    }
}