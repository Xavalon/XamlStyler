using System.Xml.Linq;

namespace XamlStyler.Core.Reorder
{
    public interface IProcessElementService
    {
        void ProcessElement(XElement element);
    }
}