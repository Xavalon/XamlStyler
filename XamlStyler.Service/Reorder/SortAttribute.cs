using System;
using System.Linq;
using System.Xml.Linq;

namespace XamlStyler.Core.Reorder
{
    public class SortAttribute
    {
        public NameMatch Name;
        public bool IsNumeric;
        public Func<XElement,string> DefaultValue;

        public SortAttribute(string nameWildcard, string namespaceWildcard, bool isNumeric, Func<XElement,string> defaultValue)
        {
            Name = new NameMatch(nameWildcard, namespaceWildcard);
            IsNumeric = isNumeric;
            DefaultValue = defaultValue;
        }

        public SortAttribute(string nameWildcard, string namespaceWildcard, bool isNumeric)
            : this(nameWildcard, namespaceWildcard, isNumeric, x => "")
        {
        }

        public ISortableAttribute GetValue(XElement element)
        {
            var attribute = element.Attributes().FirstOrDefault(x => Name.IsMatch(x.Name));
            string value = null;
            if (attribute != null)
            {
                value = attribute.Value;
            }

            return IsNumeric 
                ? (ISortableAttribute) new SortableNumericAttribute(value, Double.Parse(DefaultValue(element)))
                : (ISortableAttribute) new SortableStringAttribute(value ?? DefaultValue(element));
        }
    }
}