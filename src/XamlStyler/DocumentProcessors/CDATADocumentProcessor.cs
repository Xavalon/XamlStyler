// (c) Xavalon. All rights reserved.

using System;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Extensions;
using Xavalon.XamlStyler.Parser;
using Xavalon.XamlStyler.Services;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal class CDATADocumentProcessor : IDocumentProcessor
    {
        private readonly IndentService indentService;

        public CDATADocumentProcessor(IndentService indentService)
        {
            this.indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            // If there is linefeed(s) between element and CDATA then treat CDATA as element and 
            // indent accordingly, otherwise treat as single line text.
            if (output.IsNewLine())
            {
                elementProcessContext.UpdateParentElementProcessStatus(ContentTypes.MultiLineTextOnly);
                if (!elementProcessContext.Current.IsPreservingSpace)
                {
                    string currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);
                    output.Append(currentIndentString);
                }
            }
            else
            {
                elementProcessContext.UpdateParentElementProcessStatus(ContentTypes.SingleLineTextOnly);
            }

            // All newlines are returned by XmlReader as '\n' due to requirements in the XML Specification.
            // http://www.w3.org/TR/2008/REC-xml-20081126/#sec-line-ends
            // Change them back into the environment newline characters.
            output.Append("<![CDATA[")
                .Append(xmlReader.Value.Replace("\n", Environment.NewLine))
                .Append("]]>");
        }
    }
}