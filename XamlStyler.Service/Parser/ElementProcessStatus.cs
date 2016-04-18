namespace XamlStyler.Core.Parser
{
    public class ElementProcessStatus
    {
        /// <summary>
        /// Gets or sets the content type of current element.
        /// E.g.,
        ///     <Element></Element> : ContentTypeEnum.NONE
        ///     <Element>asdf<OtherElements/></Element> ContentTypeEnum.TEXT_ONLY
        ///     <Element>asdf<OtherElements/></Element> ContentTypeEnum.MIXED
        /// </summary>
        public ContentTypeEnum ContentType { get; set; }

        /// <summary>
        /// Gets or sets whether the start tag of this element has been broken into multi-lines.
        /// </summary>
        public bool IsMultlineStartTag { get; set; }

        /// <summary>
        /// Gets or sets whether the current element is self-closing.
        /// E.g., <Element/> is an self-closing element.
        /// </summary>
        public bool IsSelfClosingElement { get; set; }

        /// <summary>
        /// Gets or sets whether the current element preserves space.
        /// </summary>
        public bool IsPreservingSpace { get; set; }

        /// <summary>
        /// Are we currently processing significant whitespace
        /// </summary>
        public bool IsSignificantWhiteSpace { get; set; }

        /// <summary>
        /// Gets or sets Element name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Access to parent element
        /// </summary>
        public ElementProcessStatus Parent { get; set; }

    }
}