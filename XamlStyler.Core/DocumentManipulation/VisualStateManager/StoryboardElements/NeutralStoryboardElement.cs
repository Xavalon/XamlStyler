// © Xavalon. All rights reserved.

using System.Xml.Linq;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class NeutralStoryboardElement : StoryboardElement
    {
        public NeutralStoryboardElement(XElement element) : base(element)
        {
            this.ConvertableStatus = ConvertableStatus.Neutral;
        }
    }
}