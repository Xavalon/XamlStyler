// (c) Xavalon. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xavalon.XamlStyler.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.MarkupExtensions.Formatter
{
    internal class MultiLineMarkupExtensionFormatter : MarkupExtensionFormatterBase
    {
        internal MultiLineMarkupExtensionFormatter(MarkupExtensionFormatter markupExtensionFormatter)
            : base(markupExtensionFormatter)
        {
        }

        protected override IEnumerable<string> FormatArguments(Collection<Argument> arguments, bool isNested = false)
        {
            var list = new List<string>();
            string deferred = null;
            foreach (var argument in arguments)
            {
                if (deferred != null)
                {
                    deferred += ",";
                }

                foreach (var line in this.FormatArgument(argument, isNested: isNested))
                {
                    if (deferred != null)
                    {
                        list.Add(deferred);
                    }

                    deferred = line;
                }
            }

            if (deferred != null)
            {
                list.Add(deferred);
            }

            return list;
        }
    }
}