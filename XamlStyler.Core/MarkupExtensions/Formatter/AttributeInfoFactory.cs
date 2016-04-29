using System.Xml;
using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;
using Xavalon.XamlStyler.Core.Model;

namespace Xavalon.XamlStyler.Core.MarkupExtensions.Formatter
{
    public class AttributeInfoFactory
    {
        private readonly AttributeOrderRules _orderRules;
        private readonly MarkupExtensionParser _parser;
    
        public AttributeInfoFactory(MarkupExtensionParser parser, AttributeOrderRules orderRules)
        {
            _parser = parser;
            _orderRules = orderRules;
        }

        public AttributeInfo Create(XmlReader xmlReader)
        {
            string attributeName = xmlReader.Name;
            string attributeValue = xmlReader.Value;
            AttributeOrderRule orderRule = _orderRules.GetRuleFor(attributeName);
            MarkupExtension markupExtension = ParseMarkupExtension(attributeValue);

            return new AttributeInfo(attributeName, attributeValue, orderRule, markupExtension);
        }

        private  MarkupExtension ParseMarkupExtension(string value)
        {
            // Only try to parse if there is a chance that it is a markup extension
            if (value.IndexOf('{') != -1)
            {
                MarkupExtension markupExtension;
                if (_parser.TryParse(value, out markupExtension))
                    return markupExtension;
            }
            return null;
        }
    }
}