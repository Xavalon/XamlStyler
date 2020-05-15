// (c) Xavalon. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Xavalon.XamlStyler.DocumentManipulation
{
    // TODO: Fully implement IComparable interface.
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "No clear ROI and introduces more warnings")]
    public class SortableStringAttribute : ISortableAttribute
    {
        public string Value { get; private set; }

        public SortableStringAttribute(string value)
        {
            this.Value = value;
        }

        public int CompareTo(ISortableAttribute other)
        {
            return String.Compare(this.Value, ((SortableStringAttribute)other).Value, StringComparison.Ordinal);
        }

#if DEBUG
        public override string ToString()
        {
            return this.Value;
        }
#endif
    }
}