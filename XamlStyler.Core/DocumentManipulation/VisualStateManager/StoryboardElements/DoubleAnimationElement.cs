// © Xavalon. All rights reserved.

using System;
using System.Linq;
using System.Xml.Linq;
using Xavalon.XamlStyler.Core.Extensions;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public sealed class DoubleAnimationElement : StoryboardElement
    {
        public DoubleAnimationElement(XElement element) : base(element)
        {
            if (this.attributes.Any(_ => _.MatchesAny(this.DurationAttributes)))
            {
                this.ConvertableStatus = ConvertableStatus.Convertable;
            }
        }

        public override string GetSetterTarget()
        {
            return this.GetStoryboardTargetNameAndProperty();
        }

        public override string GetSetterValue()
        {
            return this.attributes.FindAttribute("To")?.Value ?? String.Empty;
        }
    }
}
