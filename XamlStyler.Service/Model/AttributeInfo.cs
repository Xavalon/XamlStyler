using System;
using System.Text.RegularExpressions;

namespace XamlStyler.Core.Model
{
    public class AttributeInfo : IComparable
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

        int IComparable.CompareTo(object obj)
        {
            var target = obj as AttributeInfo;

            if (target == null)
            {
                return 0;
            }

            if (OrderRule.AttributeTokenType != target.OrderRule.AttributeTokenType)
            {
                return OrderRule.AttributeTokenType.CompareTo(target.OrderRule.AttributeTokenType);
            }

            if (OrderRule.Priority != target.OrderRule.Priority)
            {
                return OrderRule.Priority.CompareTo(target.OrderRule.Priority);
            }

            return String.Compare(Name, target.Name, StringComparison.Ordinal);
        }
    }
}