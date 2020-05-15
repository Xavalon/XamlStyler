// (c) Xavalon. All rights reserved.

using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.DocumentManipulation
{
    public class AttributeSelector : NameSelector
    {
        private Regex valueRegex;

        private string value;
        [DisplayName("Value")]
        [Description("Match value by name. null/empty = all. 'DOS' Wildcards permitted.")]
        public string Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.valueRegex = (this.value != null) ? new Wildcard(this.value) : null;
            }
        }

        public AttributeSelector(string name, string value)
            : base(name)
        {
            this.Value = value;
        }

        public AttributeSelector(string name, string @namespace, string value)
            : base(name, @namespace)
        {
            this.Value = value;
        }

        public bool IsMatch(XAttribute attribute)
        {
            if (!this.valueRegex?.IsMatch(attribute.Value) ?? false)
            {
                return false;
            }

            return base.IsMatch(attribute.Name);
        }

        public override string ToString()
        {
            var prefix = ((this.Namespace != null) ? $"{this.Namespace}:" : String.Empty);
            return $"{prefix}{this.Name}={this.Value}";
        }
    }
}