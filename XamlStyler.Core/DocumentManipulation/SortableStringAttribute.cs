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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(SortableStringAttribute left, SortableStringAttribute right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(SortableStringAttribute left, SortableStringAttribute right)
        {
            return !(left == right);
        }

        public static bool operator <(SortableStringAttribute left, SortableStringAttribute right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(SortableStringAttribute left, SortableStringAttribute right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(SortableStringAttribute left, SortableStringAttribute right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(SortableStringAttribute left, SortableStringAttribute right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }

#if DEBUG
        public override string ToString()
        {
            return this.Value;
        }
#endif
    }
}