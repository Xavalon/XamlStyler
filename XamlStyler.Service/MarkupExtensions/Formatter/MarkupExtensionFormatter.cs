using System.Collections.Generic;
using System.Linq;
using XamlStyler.Core.MarkupExtensions.Parser;

namespace XamlStyler.Core.MarkupExtensions.Formatter
{
    public class MarkupExtensionFormatter
    {
        private readonly IList<string> _singleLineTypes;
        readonly MarkupExtensionFormatterBase _singleLineFormatter = new SingleLineMarkupExtensionFormatter();
        readonly MarkupExtensionFormatterBase _multiLineFormatter = new MultiLineMarkupExtensionFormatter();

        public MarkupExtensionFormatter(IList<string> singleLineTypes)
        {
            _singleLineTypes = singleLineTypes;
        }

        /// <summary>
        /// Format markup extension and return elements as formatted lines with "local" indention.
        /// Indention from previous element/attribute/tags must be applied separately
        /// </summary>
        /// <param name="markupExtension"></param>
        /// <returns></returns>
        public IEnumerable<string> Format(MarkupExtension markupExtension)
        {
            var formatter =
                (_singleLineTypes.Contains(markupExtension.TypeName))
                    ? _singleLineFormatter
                    : _multiLineFormatter;
            return formatter.Format(markupExtension);
        }

        /// <summary>
        /// Format markup extension on a single line.
        /// </summary>
        /// <param name="markupExtension"></param>
        /// <returns></returns>
        public string FormatSingleLine(MarkupExtension markupExtension)
        {
            return _singleLineFormatter.Format(markupExtension).Single();
        }
    }
}