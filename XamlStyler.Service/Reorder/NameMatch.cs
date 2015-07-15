using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XamlStyler.Core.Reorder
{
    public class NameMatch
    {
        private readonly Regex _nameRegex;
        private readonly Regex _namespaceRegex;

        public NameMatch(string nameWildcard, string namespaceWildcard)
        {
            if(!string.IsNullOrWhiteSpace(nameWildcard)) _nameRegex = new Wildcard(nameWildcard);
            if(!string.IsNullOrWhiteSpace(namespaceWildcard)) _namespaceRegex = new Wildcard(namespaceWildcard);
        }

        public bool IsMatch(XName name)
        {
            if (_nameRegex != null && !_nameRegex.IsMatch(name.LocalName)) return false;
            if (_namespaceRegex != null && !_namespaceRegex.IsMatch(name.Namespace.NamespaceName)) return false;
            return true;
        }
    }
}