// © Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xavalon.XamlStyler.Core.Extensions;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class StoryboardElement
    {
        protected readonly XAttribute[] KeyTimeAttributes = new XAttribute[]
        {
            new XAttribute("KeyTime", "0"),
            new XAttribute("KeyTime", "0:0:0"),
        };

        protected readonly XAttribute[] DurationAttributes = new XAttribute[]
        {
            new XAttribute("Duration", "0"),
            new XAttribute("Duration", "0:0:0"),
        };

        protected readonly XElement keyFrame;
        protected readonly IEnumerable<XAttribute> attributes;

        public readonly XElement Element;

        // Storyboards are assumed to not be convertable until proven otherwise.
        public ConvertableStatus ConvertableStatus { get; protected set; } = ConvertableStatus.NotConvertable;

        public StoryboardElement(XElement element)
        {
            this.Element = element;

            try
            {
                this.keyFrame = element.Descendants().Single();
            }
            catch (Exception)
            {
                // Ignore.
            };
            
            this.attributes = element.Attributes();
        }

        public XElement GetSetter()
        {
            var setter = new XElement(this.Element.Parent.GetDefaultNamespace() + "Setter");
            setter.SetAttributeValue("Target", this.GetSetterTarget());
            setter.SetAttributeValue("Value", this.GetSetterValue());
            return setter;
        }

        public virtual string GetSetterTarget()
        {
            if (this.ConvertableStatus == ConvertableStatus.NotConvertable)
            {
                throw new InvalidOperationException("Storyboard Element not convertable");
            }

            return String.Empty;
        }

        public virtual string GetSetterValue()
        {
            if (this.ConvertableStatus == ConvertableStatus.NotConvertable)
            {
                throw new InvalidOperationException("Storyboard Element not convertable");
            }

            return String.Empty;
        }

        protected string GetStoryboardTargetNameAndProperty()
        {
            var targetName = this.attributes.FindAttribute("Storyboard.TargetName")?.Value ?? String.Empty;
            var targetProperty = this.attributes.FindAttribute("Storyboard.TargetProperty")?.Value ?? String.Empty;
            return $"{targetName}.{targetProperty}";
        }
    }

    public enum ConvertableStatus
    {
        Convertable,
        Neutral,
        NotConvertable,
    }
}
