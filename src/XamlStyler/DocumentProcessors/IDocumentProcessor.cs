// (c) Xavalon. All rights reserved.

using System.Text;
using System.Xml;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal interface IDocumentProcessor
    {
        void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext);
    }
}