// (c) Xavalon. All rights reserved.

using System.Text;
using System.Xml;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal class XmlDeclarationDocumentProcessor : IDocumentProcessor
    {
        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            output.Append("<?xml ");
            output.Append(xmlReader.Value.Trim());
            output.Append(" ?>");
        }
    }
}