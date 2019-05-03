// © Xavalon. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xavalon.XamlStyler.Core.Extensions;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class FormatThicknessService : IProcessElementService
    {
        private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private static readonly XName SetterName = XName.Get("Setter", XamlNamespace);

        public FormatThicknessService(ThicknessStyle thicknessStyle, string thicknessAttributes)
        {
            this.IsEnabled = (thicknessStyle != ThicknessStyle.None);
            this.ThicknessStyle = thicknessStyle;
            this.ThicknessAttributeNames = thicknessAttributes.ToNameSelectorList();
        }

        public bool IsEnabled { get; }

        public ThicknessStyle ThicknessStyle { get; }

        public IList<NameSelector> ThicknessAttributeNames { get; }

        public void ProcessElement(XElement element)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            if (!element.HasAttributes)
            {
                return;
            }

            // Setter? Format "Value" attribute if "Property" attribute matches ThicknessAttributeNames
            if (element.Name == SetterName)
            {
                var propertyAttribute = element.Attributes("Property").FirstOrDefault();
                if ((propertyAttribute != null) && !propertyAttribute.Value.Contains(":")
                    && this.ThicknessAttributeNames.Any(_ => _.IsMatch(propertyAttribute.Value)))
                {
                    var valueAttribute = element.Attributes("Value").FirstOrDefault();
                    if (valueAttribute != null)
                    {
                        this.FormatAttribute(valueAttribute);
                    }
                }
            }
            else
            {
                // Not setter. Format value of all attributes where attribute name matches ThicknessAttributeNames
                foreach (var attribute in element.Attributes())
                {
                    var isMatchingAttribute = this.ThicknessAttributeNames.Any(_ => _.IsMatch(attribute.Name));
                    if (isMatchingAttribute)
                    {
                        this.FormatAttribute(attribute);
                    }
                }
            }
        }

        private void FormatAttribute(XAttribute attribute)
        {
            char separator = (ThicknessStyle == ThicknessStyle.Comma) ? ',' : ' ';

            string formatted;
            if (ThicknessFormatter.TryFormat(attribute.Value, separator, out formatted))
            {
                attribute.Value = formatted;
            }
        }
    }
}