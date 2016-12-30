// © Xavalon. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Xavalon.XamlStyler.Core.Extensions;
using Xavalon.XamlStyler.Core.Model;
using Xavalon.XamlStyler.Core.Services;

namespace Xavalon.XamlStyler.Core.MarkupExtensions.Formatter
{
    public class AttributeInfoFormatter
    {
        private readonly MarkupExtensionFormatter formatter;
        private readonly IndentService indentService;

        public AttributeInfoFormatter(MarkupExtensionFormatter formatter, IndentService indentService)
        {
            this.formatter = formatter;
            this.indentService = indentService;
        }

        /// <summary>
        /// Handles markup extension value in style as:
        /// XyzAttribute="{XyzMarkup value1,
        ///                          value2,
        ///                          key1=value1,
        ///                          key2=value2}"
        /// </summary>
        /// <param name="attributeInfo"></param>
        /// <param name="baseIndentationString"></param>
        /// <returns></returns>
        public string ToMultiLineString(AttributeInfo attributeInfo, string baseIndentationString)
        {
            if (!attributeInfo.IsMarkupExtension)
            {
                throw new ArgumentException("AttributeInfo shall have a markup extension value.",
                                            MethodBase.GetCurrentMethod().GetParameters()[0].Name);
            }

            if (attributeInfo.IsMarkupExtension)
            {
                var currentIndentationString = $"{baseIndentationString}{string.Empty.PadLeft(attributeInfo.Name.Length + 2, ' ')}";
                var lines = this.formatter.Format(attributeInfo.MarkupExtension);

                var buffer = new StringBuilder();
                buffer.AppendFormat("{0}=\"{1}", attributeInfo.Name, lines.First());
                foreach (var line in lines.Skip(1))
                {
                    buffer.AppendLine();
                    buffer.Append(this.indentService.Normalize(currentIndentationString + line));
                }

                buffer.Append('"');
                return buffer.ToString();
            }

            return $"{attributeInfo.Name}=\"{attributeInfo.Value}\"";
        }

        /// <summary>
        /// Single line value line in style as:
        /// attribute_name="attribute_value"
        /// </summary>
        /// <param name="attributeInfo"></param>
        /// <returns></returns>
        public string ToSingleLineString(AttributeInfo attributeInfo)
        {
            var valuePart = attributeInfo.IsMarkupExtension ? this.formatter.FormatSingleLine(attributeInfo.MarkupExtension)
                                                            : attributeInfo.Value.ToXmlEncodedString();

            return $"{attributeInfo.Name}=\"{valuePart}\"";
        }
    }
}