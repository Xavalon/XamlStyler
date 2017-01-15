// © Xavalon. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.Core.MarkupExtensions.Formatter
{
    public class MarkupExtensionFormatter
    {
        private readonly IList<string> singleLineTypes;
        private readonly MarkupExtensionFormatterBase singleLineFormatter;
        private readonly MarkupExtensionFormatterBase multiLineFormatter;

        public MarkupExtensionFormatter(IList<string> singleLineTypes)
        {
            this.singleLineTypes = singleLineTypes;

            this.singleLineFormatter = new SingleLineMarkupExtensionFormatter(this);
            this.multiLineFormatter = new MultiLineMarkupExtensionFormatter(this);
        }

        /// <summary>
        /// Format markup extension and return elements as formatted lines with "local" indention.
        /// Indention from previous element/attribute/tags must be applied separately
        /// </summary>
        /// <param name="markupExtension"></param>
        /// <returns></returns>
        public IEnumerable<string> Format(MarkupExtension markupExtension)
        {
            var formatter = (this.singleLineTypes.Contains(markupExtension.TypeName))
                ? this.singleLineFormatter
                : this.multiLineFormatter;
            return formatter.Format(markupExtension);
        }

        /// <summary>
        /// Format markup extension on a single line.
        /// </summary>
        /// <param name="markupExtension"></param>
        /// <returns></returns>
        public string FormatSingleLine(MarkupExtension markupExtension)
        {
            return this.singleLineFormatter.Format(markupExtension).Single();
        }
    }
}