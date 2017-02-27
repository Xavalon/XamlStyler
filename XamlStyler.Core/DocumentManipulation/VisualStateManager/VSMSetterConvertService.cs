// © Xavalon. All rights reserved.

using System.Linq;
using System.Xml.Linq;
using Xavalon.XamlStyler.Core.Extensions;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class VSMSetterConvertService : IProcessElementService
    {
        public readonly NameSelector VisualStateNode = new NameSelector("VisualState");
        public readonly NameSelector VisualStateSettersNode = new NameSelector("VisualState.Setters");
        public readonly NameSelector StoryboardNode = new NameSelector("Storyboard");

        public bool IsEnabled { get; set; } = false;

        public void ProcessElement(XElement element)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            if (!element.HasElements)
            {
                return;
            }

            if (this.VisualStateNode.IsMatch(element.Name))
            {
                this.ConvertStoryboardToSetters(element);
            }
        }

        private void ConvertStoryboardToSetters(XElement visualStateElement)
        {
            if (visualStateElement == null)
            {
                return;
            }

            // Retreive the Storyboard element.
            var storyboardNode = visualStateElement.Descendants().FirstOrDefault(_ => this.StoryboardNode.IsMatch(_.Name));
            if (storyboardNode == null)
            {
                return;
            }

            // Retrieve the existing Setters element, else create a new one.
            bool insertSettersNode = false;
            var settersNode = visualStateElement.Descendants().FirstOrDefault(_ => this.VisualStateSettersNode.IsMatch(_.Name));
            if (settersNode == null)
            {
                insertSettersNode = true;
                settersNode = new XElement(visualStateElement.GetDefaultNamespace() + "VisualState.Setters");
            }

            var storyboardElements = storyboardNode.Elements().Select(_ => StoryboardElementFactory.GetElement(_));

            // Process storyboards unless they have elements that are not convertable.
            if (!storyboardElements.Any(_ => _.ConvertableStatus == ConvertableStatus.NotConvertable))
            {
                // Only convert convertable elements (ignore neutral elements).
                var convertList = storyboardElements.Where(_ => _.ConvertableStatus == ConvertableStatus.Convertable).ToList();
                foreach(var node in convertList)
                {
                    settersNode.Add(node.GetSetter());
                    node.Element.RemoveWithTrailingWhitespace();
                }

                // Add Setters element if it has elements.
                if (insertSettersNode && settersNode.HasElements)
                {
                    visualStateElement.AddFirst(settersNode);
                }

                // Remove empty Storyboard elements.
                if (!storyboardNode.HasElements)
                {
                    storyboardNode.RemoveWithTrailingWhitespace();
                }
            }
        }
    }
}