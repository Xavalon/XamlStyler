// (c) Xavalon. All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.DocumentManipulation
{
    public class SortBy : NameSelector
    {
        private bool isNumeric;
        private Func<XElement, string> defaultValue;

        public SortBy(string name, string @namespace, bool isNumeric)
            : base(name, @namespace)
        {
            this.IsNumeric = isNumeric;
        }

        public SortBy(string name, bool isNumeric)
            : base(name)
        {
            this.IsNumeric = isNumeric;
        }

        [DisplayName("Namespace")]
        [Description("Match name by namespace. null/empty = all. 'DOS' Wildcards permitted.")]
        public bool IsNumeric
        {
            get { return this.isNumeric; }
            set
            {
                this.isNumeric = value;
                this.defaultValue = this.IsNumeric
                    ? (Func<XElement, string>)(_ => _.Name.LocalName.Contains(".") ? "-32768" : "-32767")
                    : (x => String.Empty);
            }
        }

        public ISortableAttribute GetValue(XElement element)
        {
            var attribute = element.Attributes().FirstOrDefault(_ => IsMatch(_.Name));
            string value = null;
            if (attribute != null)
            {
                value = attribute.Value;
            }

            return this.IsNumeric
                ? (ISortableAttribute)new SortableNumericAttribute(value, Double.Parse(this.defaultValue(element), CultureInfo.InvariantCulture))
                : (ISortableAttribute)new SortableStringAttribute(value ?? this.defaultValue(element));
        }
    }
}