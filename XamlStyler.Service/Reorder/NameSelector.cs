using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XamlStyler.Core.Reorder
{
    public class NameSelector
    {
        private string _name;
        private Regex _nameRegex;
        private string _namespace;
        private Regex _namespaceRegex;

        [DisplayName("Name")]
        [Description("Match name by name. null/empty = all. 'DOS' Wildcards permitted.")]
        public string Name
        { 
            get { return _name; }
            set
            {
                _name = value;
                _nameRegex = _name != null ? new Wildcard(_name) : null;
            } 
        }

        [DisplayName("Namespace")]
        [Description("Match name by namespace. null/empty = all. 'DOS' Wildcards permitted.")]
        public string Namespace
        { 
            get { return _namespace; }
            set
            {
                _namespace = value;
                _namespaceRegex = _namespace != null ? new Wildcard(_namespace) : null;
            } 
        }

        public NameSelector()
        {
        }

        public NameSelector(string name)
        {
            Name = name;
        }

        public NameSelector(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }

        public bool IsMatch(XName name)
        {
            if (_nameRegex != null && !_nameRegex.IsMatch(name.LocalName)) return false;
            if (_namespaceRegex != null && !_namespaceRegex.IsMatch(name.Namespace.NamespaceName)) return false;
            return true;
        }

        public override string ToString()
        {
            return
                (Namespace != null ? Namespace + ":" : null) + Name;
        }
    }
}