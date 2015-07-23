using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace XamlStyler.Core.Reorder
{
    public class SortBy: NameSelector
    {
        private bool _isNumeric;
        private Func<XElement,string> _defaultValue;

        public SortBy(string name, string @namespace, bool isNumeric)
            : base(name, @namespace)
        {
            IsNumeric = isNumeric;
        }

        public SortBy(string name, bool isNumeric)
            : base(name)
        {
            IsNumeric = isNumeric;
        }

        [DisplayName("Namespace")]
        [Description("Match name by namespace. null/empty = all. 'DOS' Wildcards permitted.")]
        public bool IsNumeric { 
            get { return _isNumeric; }
            set
            {
                _isNumeric = value;
                _defaultValue = IsNumeric
                    ? (Func<XElement, string>) (x => x.Name.LocalName.Contains(".") ? "-32768" : "-32767")
                    : (x => "");
            } 
        }

        public ISortableAttribute GetValue(XElement element)
        {
            var attribute = element.Attributes().FirstOrDefault(x => IsMatch(x.Name));
            string value = null;
            if (attribute != null)
            {
                value = attribute.Value;
            }

            return IsNumeric 
                ? (ISortableAttribute) new SortableNumericAttribute(value, Double.Parse(_defaultValue(element)))
                : (ISortableAttribute) new SortableStringAttribute(value ?? _defaultValue(element));
        }
    }
}