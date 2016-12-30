// © Xavalon. All rights reserved.

using System;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Core.Extensions;
using Xavalon.XamlStyler.Core.Options;
using Xavalon.XamlStyler.Core.Parser;
using Xavalon.XamlStyler.Core.Services;

namespace Xavalon.XamlStyler.Core.DocumentProcessors
{
    internal class EndElementDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions options;
        private readonly IndentService indentService;

        public EndElementDocumentProcessor(IStylerOptions options, IndentService indentService)
        {
            this.indentService = indentService;
            this.options = options;
        }

        public void Process(XmlReader xmlReader,
                            StringBuilder output,
                            ElementProcessContext elementProcessContext)
        {
            if (elementProcessContext.Current.IsPreservingSpace)
            {
                output.Append("</").Append(xmlReader.Name).Append(">");
            }
            else if (elementProcessContext.Current.IsSignificantWhiteSpace && !output.IsNewLine())
            {
                output.Append("</").Append(xmlReader.Name).Append(">");
            }
            else if ((elementProcessContext.Current.ContentType == ContentTypeEnum.None) && this.options.RemoveEndingTagOfEmptyElement)
            {
                // Shrink the current element, if it has no content.
                // E.g., <Element>  </Element> => <Element />
                output = output.TrimEnd(' ', '\t', '\r', '\n');

                int bracketIndex = output.LastIndexOf('>');
                output.Insert(bracketIndex, '/');

                if ((output[bracketIndex - 1] != '\t')
                    && (output[bracketIndex - 1] != ' ')
                    && this.options.SpaceBeforeClosingSlash)
                {
                    output.Insert(bracketIndex, ' ');
                }
            }
            else if ((elementProcessContext.Current.ContentType == ContentTypeEnum.SingleLineTextOnly)
                     && !elementProcessContext.Current.IsMultlineStartTag)
            {
                var bracketIndex = output.LastIndexOf('>');
                var text = output.Substring((bracketIndex + 1), (output.Length - bracketIndex - 1)).Trim();

                output.Length = (bracketIndex + 1);
                output.Append(text).Append("</").Append(xmlReader.Name).Append(">");
            }
            else
            {
                var currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);
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