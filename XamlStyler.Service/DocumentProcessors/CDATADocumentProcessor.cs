using System;
using System.Text;
using System.Xml;
using XamlStyler.Core.Extensions;
using XamlStyler.Core.Parser;
using XamlStyler.Core.Services;

namespace XamlStyler.Core.DocumentProcessors
{
    internal class CDATADocumentProcessor : IDocumentProcessor
    {
        private readonly IndentService _indentService;

        public CDATADocumentProcessor(IndentService indentService)
        {
            _indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            // If there is linefeed(s) between element and CDATA then treat CDATA as element and indent accordingly, otherwise treat as single line text
            if (output.IsNewLine())
            {
                elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.MULTI_LINE_TEXT_ONLY);
                if (!elementProcessContext.Current.IsPreservingSpace)
                {
                    string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);
                    output.Append(currentIndentString);
                }
            }
            else
            {
                elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.SINGLE_LINE_TEXT_ONLY);
            }

            output.Append("<![CDATA[")
                // All newlines are returned by XmlReader as \n due to requirements in the XML Specification (http://www.w3.org/TR/2008/REC-xml-20081126/#sec-line-ends)
                // Change them back into the environment newline characters.
                .Append(xmlReader.Value.Replace("\n", Environment.NewLine)).Append("]]>");
        }
    }
}