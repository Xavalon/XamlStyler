// © Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
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