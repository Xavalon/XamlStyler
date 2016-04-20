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

        /*
        <TextBlock HorizontalAlignment="Center" 
               VerticalAlignment="Center"
               FontSize="20"
               Foreground="Red"
               Text="{Binding {}Title,
                              RelativeSource={RelativeSource FindAncestor,
                                                             AncestorType={x:Type Page}},
                              StringFormat={}{0}Now{{0}} - {0}}" />

        <TextBlock Text="{Binding Path=Content, ElementName=m_button, StringFormat={}{0:##\,#0.00;(##\,#0.00); }}" HorizontalAlignment="Right" VerticalAlignment="Top"/>

        */
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


        //public string FormatMultiLineString(AttributeInfo attrInfo, string baseIndentationString)
        //{
        //    if(_parser.TryParse(attrInfo.MarkupExtension.))
        //}
        //public string AttributeInfoFactory(string attribute)
        //{
        //    MarkupExtension markupExtension;
        //    if(_parser.TryParse(attribute, out markupExtension))
        //}
        public string FormatSingleLine(MarkupExtension graph)
        {
            return _singleLineFormatter.Format(graph).Single();
        }
    }
}