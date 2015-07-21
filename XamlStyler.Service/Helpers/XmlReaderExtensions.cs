using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XamlStyler.Core.Helpers
{
    internal static class XmlReaderExtensions
    {
        /// <summary>
        /// Check for xml:space as defined in http://www.w3.org/TR/2008/REC-xml-20081126/#sec-white-space
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <returns>true if xml:space</returns>
        public static bool IsXmlSpaceAttribute(this XmlReader xmlReader)
        {
            return (xmlReader.Name.ToLowerInvariant() == "xml:space"); 
        }
    }
}
