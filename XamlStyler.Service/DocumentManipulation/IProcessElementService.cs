using System.Xml.Linq;

namespace XamlStyler.Core.DocumentManipulation
{
    public interface IProcessElementService
    {
        void ProcessElement(XElement element);
    }
}