using System;

namespace XamlStyler.Core.DocumentManipulation
{
    public class SortableStringAttribute: ISortableAttribute
    {
        public string Value { get; private set; }

        public SortableStringAttribute(string value)
        {
            Value = value;
        }

        public int CompareTo(ISortableAttribute other)
        {
            return String.Compare(Value, ((SortableStringAttribute) other).Value, StringComparison.Ordinal);
        }

#if DEBUG
        public override string ToString()
        {
            return Value;
        }
#endif
    }
}