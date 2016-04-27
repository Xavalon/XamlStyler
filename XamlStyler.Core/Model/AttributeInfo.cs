using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.Core.Model
{
    public class AttributeInfo
    {
        public AttributeOrderRule OrderRule { get; }
        public string Name { get; }
        public string Value { get; }
        public MarkupExtension MarkupExtension { get; }
        public bool IsMarkupExtension => MarkupExtension != null;

        public AttributeInfo(string name, string value, AttributeOrderRule orderRule, MarkupExtension markupExtension)
        {
            Name = name;
            Value = value;
            OrderRule = orderRule;
            MarkupExtension = markupExtension;
        }
    }
}