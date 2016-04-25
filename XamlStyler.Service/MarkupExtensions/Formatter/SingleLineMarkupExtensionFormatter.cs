using System.Collections.Generic;
using System.Text;
using XamlStyler.Core.MarkupExtensions.Parser;

namespace XamlStyler.Core.MarkupExtensions.Formatter
{
    internal class SingleLineMarkupExtensionFormatter : MarkupExtensionFormatterBase
    {
        protected override IEnumerable<string> Format(Argument[] arguments)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var argument in arguments)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                foreach (var line in Format(argument))
                {
                    sb.Append(line);
                }
            }
            yield return sb.ToString();
        }
    }
}