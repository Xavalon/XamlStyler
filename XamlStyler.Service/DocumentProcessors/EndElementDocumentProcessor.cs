using System;
using System.Text;
using System.Xml;
using XamlStyler.Core.Extensions;
using XamlStyler.Core.Options;
using XamlStyler.Core.Parser;
using XamlStyler.Core.Services;

namespace XamlStyler.Core.DocumentProcessors
{
    internal class EndElementDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions _options;
        private readonly IndentService _indentService;

        public EndElementDocumentProcessor(IStylerOptions options,IndentService indentService)
        {
            _indentService = indentService;
            _options = options;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            if (elementProcessContext.Current.IsPreservingSpace)
            {
                output.Append("</").Append(xmlReader.Name).Append(">");
            }
            else if (elementProcessContext.Current.IsSignificantWhiteSpace && !output.IsNewLine())
            {
                output.Append("</").Append(xmlReader.Name).Append(">");
            }
            // Shrink the current element, if it has no content.
            // E.g., <Element>  </Element> => <Element />
            else if (elementProcessContext.Current.ContentType == ContentTypeEnum.NONE && _options.RemoveEndingTagOfEmptyElement)
            {
                #region shrink element with no content

                output = output.TrimEnd(' ', '\t', '\r', '\n');

                int bracketIndex = output.LastIndexOf('>');
                output.Insert(bracketIndex, '/');

                if (output[bracketIndex - 1] != '\t' && output[bracketIndex - 1] != ' ' && _options.SpaceBeforeClosingSlash)
                {
                    output.Insert(bracketIndex, ' ');
                }

                #endregion shrink element with no content
            }
            else if (elementProcessContext.Current.ContentType == ContentTypeEnum.SINGLE_LINE_TEXT_ONLY && elementProcessContext.Current.IsMultlineStartTag == false)
            {
                int bracketIndex = output.LastIndexOf('>');

                string text = output.Substring(bracketIndex + 1, output.Length - bracketIndex - 1).Trim();

                output.Length = bracketIndex + 1;
                output.Append(text).Append("</").Append(xmlReader.Name).Append(">");
            }
            else
            {
                string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);

                if (!output.IsNewLine())
                {
                    output.Append(Environment.NewLine);
                }

                output.Append(currentIndentString).Append("</").Append(xmlReader.Name).Append(">");
            }

            elementProcessContext.Pop();
        }
    }
}