// (c) Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Extensions;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Parser;
using Xavalon.XamlStyler.Services;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal class TextDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions options;
        private readonly IndentService indentService;

        public TextDocumentProcessor(IStylerOptions options, IndentService indentService)
        {
            this.options = options;
            this.indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypes.SingleLineTextOnly);

            var xmlEncodedContent = xmlReader.Value.ToXmlEncodedString(ignoreCarrier: true);
            if (elementProcessContext.Current.IsPreservingSpace)
            {
                output.Append(xmlEncodedContent.Replace("\n", options.NewLine));
            }
            else
            {
                string currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);
                IEnumerable<string> textLines = xmlEncodedContent.Trim()
                    .Split('\n')
                    .Where(_ => (_.Trim().Length > 0))
                    .ToList();

                foreach (var line in textLines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.Length > 0)
                    {
                        output.Append(options.NewLine).Append(currentIndentString).Append(trimmedLine);
                    }
                }
            }

            if (xmlEncodedContent.Any(_ => (_ == '\n')))
            {
                elementProcessContext.UpdateParentElementProcessStatus(ContentTypes.MultiLineTextOnly);
            }
        }
    }
}