using System;
using System.Text.RegularExpressions;

namespace XamlStyler.Core.Model
{
    public class AttributeInfo
    {
        // Fields
        private static readonly Regex MarkupExtensionPattern = new Regex("^{(?!}).*}$");

        public AttributeOrderRule OrderRule { get; private set; }

        public bool IsMarkupExtension { get; private set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public AttributeInfo(string name, string value, AttributeOrderRule orderRule)
        {
            Name = name;
            Value = value;
            IsMarkupExtension = MarkupExtensionPattern.IsMatch(value);
            OrderRule = orderRule;
        }
    }
}