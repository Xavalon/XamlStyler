using System;
using System.Text.RegularExpressions;

namespace XamlStyler.Core.Model
{
    public class AttributeInfo : IComparable
    {
        // Fields
        private static readonly Regex MarkupExtensionPattern = new Regex("^{(?!}).*}$");
        private readonly AttributeOrderRule _orderRule;

        public bool IsMarkupExtension { get; private set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public AttributeInfo(string name, string value, AttributeOrderRule orderRule)
        {
            Name = name;
            Value = value;
            IsMarkupExtension = MarkupExtensionPattern.IsMatch(value);
            _orderRule = orderRule;
        }

        int IComparable.CompareTo(object obj)
        {
            var target = obj as AttributeInfo;

            if (target == null)
            {
                return 0;
            }

            if (_orderRule.AttributeTokenType != target._orderRule.AttributeTokenType)
            {
                return _orderRule.AttributeTokenType.CompareTo(target._orderRule.AttributeTokenType);
            }

            if (_orderRule.Priority != target._orderRule.Priority)
            {
                return _orderRule.Priority.CompareTo(target._orderRule.Priority);
            }

            return String.Compare(Name, target.Name, StringComparison.Ordinal);
        }
    }
}