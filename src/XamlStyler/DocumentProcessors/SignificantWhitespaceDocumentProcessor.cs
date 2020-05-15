// (c) Xavalon. All rights reserved.

using System;
using System.Text;
using System.Xml;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal class SignificantWhitespaceDocumentProcessor : IDocumentProcessor
    {
        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            // All newlines are returned by XmlReader as '\n' due to requirements in the XML Specification.
            // http://www.w3.org/TR/2008/REC-xml-20081126/#sec-line-ends
            // Change them back into the environment newline characters.
            output.Append(xmlReader.Value.Replace("\n", Environment.NewLine));
        }
    }
}