// (c) Xavalon. All rights reserved.

using System.Xml.Linq;

namespace Xavalon.XamlStyler.DocumentManipulation
{
    public interface IProcessElementService
    {
        void ProcessElement(XElement element);
    }
}