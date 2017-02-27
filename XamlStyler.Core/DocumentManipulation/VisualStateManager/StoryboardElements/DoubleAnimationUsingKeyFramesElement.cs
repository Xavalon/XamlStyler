// © Xavalon. All rights reserved.

using System;
using System.Linq;
using System.Xml.Linq;
using Xavalon.XamlStyler.Core.Extensions;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public sealed class DoubleAnimationUsingKeyFramesElement : StoryboardElement
    {
        private readonly NameSelector[] DoubleKeyFrameNodes = new NameSelector[]
        {
            new NameSelector("DiscreteDoubleKeyFrame"),
            new NameSelector("EasingDoubleKeyFrame"),
            new NameSelector("LinearDoubleKeyFrame"),
            new NameSelector("SplineDoubleKeyFrame"),
        };

        public DoubleAnimationUsingKeyFramesElement(XElement element) : base(element)
        {
            if ((this.keyFrame != null)
                && this.DoubleKeyFrameNodes.Any(_ => _.IsMatch(this.keyFrame.Name))
                && this.keyFrame.Attributes().Any(_ => _.MatchesAny(this.KeyTimeAttributes)))
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
            return this.keyFrame.Attributes().FindAttribute("Value")?.Value ?? String.Empty;
        }
    }
}
