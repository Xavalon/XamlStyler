using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XamlStyler.Core.Helpers;

namespace XamlStyler.Core.Reorder
{
    public class FormatThicknessService : IProcessElementService
    {
        public FormatThicknessService(ThicknessStyle thicknessStyle, string thicknessAttributes)
        {
            IsEnabled = thicknessStyle != ThicknessStyle.None;
            ThicknessStyle = thicknessStyle;
            ThicknessAttributeNames = thicknessAttributes.ToNameSelectorList();
        }

        public bool IsEnabled { get; }

        public ThicknessStyle ThicknessStyle { get; }
        public IList<NameSelector> ThicknessAttributeNames { get; }

        private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private static readonly XName SetterName = XName.Get("Setter", XamlNamespace);

        public void ProcessElement(XElement element)
        {
            if (!IsEnabled) return;
            if (!element.HasAttributes) return;

            // Setter? Format "Value" attribute if "Property" atribute matches ThicknessAttributeNames
            if (element.Name == SetterName)
            {
                var propertyAttribute = element.Attributes("Property").FirstOrDefault();
                if (propertyAttribute != null && ThicknessAttributeNames.Any(match => match.IsMatch(propertyAttribute.Value)))
                {
                    var valueAttribute = element.Attributes("Value").FirstOrDefault();
                    if (valueAttribute != null)
                    {
                        FormatAttribute(valueAttribute);
                    }
                }
            }
            // Not setter. Format value of all attributes where attribute name matches ThicknessAttributeNames
            else
            {
                foreach (var attribute in element.Attributes())
                {
                    var isMatchingAttribute = ThicknessAttributeNames.Any(match => match.IsMatch(attribute.Name));
                    if (isMatchingAttribute)
                    {
                        FormatAttribute(attribute);
                    }
                }
            }
        }

        private void FormatAttribute(XAttribute attribute)
        {
            char separator = ThicknessStyle == ThicknessStyle.Comma ? ',' : ' ';

            string formatted;
            if (ThicknessFormatter.TryFormat(attribute.Value, separator, out formatted))
            {
                attribute.Value = formatted;
            }
        }
    }
}