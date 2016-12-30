// © Xavalon. All rights reserved.

using System;
using System.Linq;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Core.Extensions;
using Xavalon.XamlStyler.Core.Parser;
using Xavalon.XamlStyler.Core.Services;

namespace Xavalon.XamlStyler.Core.DocumentProcessors
{
    internal class TextDocumentProcessor : IDocumentProcessor
    {
        private readonly IndentService indentService;

        public TextDocumentProcessor(IndentService indentService)
        {
            this.indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.SingleLineTextOnly);

            var xmlEncodedContent = xmlReader.Value.ToXmlEncodedString(ignoreCarrier: true);
            if (elementProcessContext.Current.IsPreservingSpace)
            {
                output.Append(xmlEncodedContent.Replace("\n", Environment.NewLine));
            }
            else
            {
                var currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);
                var textLines = xmlEncodedContent.Trim()
                                                 .Split('\n')
                                                 .Where(content => (content.Trim().Length > 0))
                                                 .ToList();

                foreach (var line in textLines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.Length > 0)
                    {
                        output.Append(Environment.NewLine).Append(currentIndentString).Append(trimmedLine);
                    }
                }
            }

            if (xmlEncodedContent.Any(content => (content == '\n')))
            {
                elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.MultiLineTextOnly);
            }
        }
    }
}