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
            this.Name = name;
            this.Value = value;
            this.OrderRule = orderRule;
            this.MarkupExtension = markupExtension;
        }
    }
}