using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class NameSelector
    {
        private string name;
        private Regex nameRegex;
        private string namespaceName;
        private Regex namespaceRegex;

        public NameSelector()
        {
        }

        public NameSelector(string name)
        {
            this.Name = name;
        }

        public NameSelector(string name, string @namespace)
        {
            this.Name = name;
            this.Namespace = @namespace;
        }

        [DisplayName("Name")]
        [Description("Match name by name. null/empty = all. 'DOS' Wildcards permitted.")]
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.nameRegex = (this.name != null) ? new Wildcard(this.name) : null;
            }
        }

        [DisplayName("Namespace")]
        [Description("Match name by namespace. null/empty = all. 'DOS' Wildcards permitted.")]
        public string Namespace
        {
            get { return this.namespaceName; }
            set
            {
                this.namespaceName = value;
                this.namespaceRegex = (this.namespaceName != null) ? new Wildcard(this.namespaceName) : null;
            }
        }

        public bool IsMatch(XName name)
        {
            if ((this.nameRegex != null) && !this.nameRegex.IsMatch(name.LocalName))
            {
                return false;
            }

            if ((this.namespaceRegex != null) && !this.namespaceRegex.IsMatch(name.Namespace.NamespaceName))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return (this.Namespace != null ? this.Namespace + ":" : null) + this.Name;
        }
    }
}