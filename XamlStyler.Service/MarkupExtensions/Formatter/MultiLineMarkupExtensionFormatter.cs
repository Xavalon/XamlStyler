using System.Collections.Generic;
using XamlStyler.Core.MarkupExtensions.Parser;

namespace XamlStyler.Core.MarkupExtensions.Formatter
{
    internal class MultiLineMarkupExtensionFormatter : MarkupExtensionFormatterBase
    {
        protected override IEnumerable<string> Format(Argument[] arguments)
        {
            var list = new List<string>();
            string deferred = null;
            foreach (var argument in arguments)
            {
                if (deferred != null)
                    deferred += ",";

                foreach (var line in Format(argument))
                {
                    if (deferred != null)
                        list.Add(deferred);
                    deferred = line;
                }
            }
            if (deferred != null)
                list.Add(deferred);
            return list;
        }
    }
}