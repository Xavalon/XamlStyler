using System.Text;
using System.Xml;

namespace XamlStyler.Core.DocumentProcessors
{
    internal interface IDocumentProcessor
    {
        void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext);
    }
}