// (c) Xavalon. All rights reserved.

using Xavalon.XamlStyler.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.Model
{
    public class AttributeInfo
    {
        public AttributeOrderRule OrderRule { get; }

        public string Name { get; }

        public string Value { get; }

        public bool AttributeHasIgnoredNamespace { get; }

        public string AttributeNameWithoutNamespace { get; }

        public MarkupExtension MarkupExtension { get; }

        public bool IsMarkupExtension => MarkupExtension != null;

        public AttributeInfo(string name, string value, bool attributeHasIgnoredNamespace, string attributeNameWithoutNamespace, AttributeOrderRule orderRule, MarkupExtension markupExtension)
        {
            this.Name = name;
            this.Value = value;
            this.AttributeHasIgnoredNamespace = attributeHasIgnoredNamespace;
            this.AttributeNameWithoutNamespace = attributeNameWithoutNamespace;
            this.OrderRule = orderRule;
            this.MarkupExtension = markupExtension;
        }
    }
}