using System;
using System.Text.RegularExpressions;

namespace XamlStyler.Core.Model
{
    public class AttributeInfo
    {
        // Fields
        private static readonly Regex MarkupExtensionPattern = new Regex(@"^{(?!}).*}$", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex MarkupTypePattern = new Regex(@"^{(?<type>[^\s}]*)", RegexOptions.Singleline | RegexOptions.Compiled);

        public AttributeOrderRule OrderRule { get; }
        public string Name { get; }
        public string Value { get; }
        public bool IsMarkupExtension { get; }
        public string MarkupExtension { get; }

        public AttributeInfo(string name, string value, AttributeOrderRule orderRule)
        {
            Name = name;
            Value = value;
            IsMarkupExtension = MarkupExtensionPattern.IsMatch(value);
            OrderRule = orderRule;

            if (IsMarkupExtension)
            {
                MatchCollection mc = MarkupTypePattern.Matches(value);
                foreach (Match m in mc)
                {
                    MarkupExtension = m.Groups["type"].Value;
                }
            }
        }
    }
}