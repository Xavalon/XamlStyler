// (c) Xavalon. All rights reserved.

using System.Collections.Generic;

namespace Xavalon.XamlStyler.Parser
{
    /// <summary>
    /// Stores information about the original format of an element in the source XAML.
    /// </summary>
    public class OriginalFormatInfo
    {
        /// <summary>
        /// Gets or sets the element name.
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// Gets or sets whether the element was originally a self-closing element (e.g., &lt;Button /&gt;).
        /// </summary>
        public bool IsSelfClosing { get; set; }

        /// <summary>
        /// Gets or sets the list of attribute indices that had line breaks before them.
        /// Index 0 means the first attribute had a line break before it (i.e., not on the same line as element name).
        /// </summary>
        public HashSet<int> AttributeLineBreakIndices { get; set; } = new HashSet<int>();

        /// <summary>
        /// Gets or sets the total number of attributes.
        /// </summary>
        public int AttributeCount { get; set; }

        /// <summary>
        /// Gets the start line number in the original source.
        /// </summary>
        public int StartLine { get; set; }

        /// <summary>
        /// Gets the start column number in the original source.
        /// </summary>
        public int StartColumn { get; set; }
    }
}
