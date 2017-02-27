// © Xavalon. All rights reserved.

using System;
using System.Linq;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public sealed class StoryboardElementFactory
    {
        // Supported Convertable Storyboard Elements
        private readonly NameSelector ObjectAnimationUsingKeyFramesNode = new NameSelector("ObjectAnimationUsingKeyFrames");
        private readonly NameSelector DoubleAnimationUsingKeyFramesNode = new NameSelector("DoubleAnimationUsingKeyFrames");
        private readonly NameSelector DoubleAnimationNode = new NameSelector("DoubleAnimation");

        private readonly NameSelector[] NeutralStoryboardElements = new NameSelector[]
        {
            new NameSelector("DragItemThemeAnimation"),
            new NameSelector("DropTargetItemThemeAnimation"),
            new NameSelector("FadeInThemeAnimation"),
            new NameSelector("FadeOutThemeAnimation"),
            new NameSelector("PointerDownThemeAnimation"),
            new NameSelector("PointerUpThemeAnimation"),
            new NameSelector("PopOutThemeAnimation")
        };

        private static Lazy<StoryboardElementFactory> instance
            = new Lazy<StoryboardElementFactory>(() => new StoryboardElementFactory());

        public static StoryboardElement GetElement(XElement element)
        {
            if (StoryboardElementFactory.instance.Value.ObjectAnimationUsingKeyFramesNode.IsMatch(element.Name))
            {
                return new ObjectAnimationUsingKeyFramesElement(element);
            }
            else if (StoryboardElementFactory.instance.Value.DoubleAnimationNode.IsMatch(element.Name))
            {
                return new DoubleAnimationElement(element);
            }
            else if (StoryboardElementFactory.instance.Value.DoubleAnimationUsingKeyFramesNode.IsMatch(element.Name))
            {
                return new DoubleAnimationUsingKeyFramesElement(element);
            }
            else
            {
                return (StoryboardElementFactory.instance.Value.NeutralStoryboardElements.Any(_ => _.IsMatch(element.Name)))
                    ? new NeutralStoryboardElement(element)
                    : new StoryboardElement(element);
            }
        }
    }
}
