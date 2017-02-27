// © Xavalon. All rights reserved.

using System;
using System.Linq;
using System.Xml.Linq;
using Xavalon.XamlStyler.Core.Extensions;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public sealed class ObjectAnimationUsingKeyFramesElement : StoryboardElement
    {
        private readonly NameSelector DiscreteObjectKeyFrameNode = new NameSelector("DiscreteObjectKeyFrame");

        public ObjectAnimationUsingKeyFramesElement(XElement element) : base(element)
        {
            if ((this.keyFrame != null)
                && this.DiscreteObjectKeyFrameNode.IsMatch(this.keyFrame.Name)
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
