using System.Xml.Linq;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public interface IProcessElementService
    {
        void ProcessElement(XElement element);
    }
}