// © Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class SortableNumericAttribute : ISortableAttribute
    {
        public string Value { get; private set; }

        public double NumericValue { get; private set; }

        public SortableNumericAttribute(string value, double defaultNumericValue)
        {
            this.Value = value;

            double numericValue;
            this.NumericValue = Double.TryParse(value, out numericValue)
                ? numericValue
                : defaultNumericValue;
        }

        public int CompareTo(ISortableAttribute other)
        {
            var otherSortableNumericAttribute = (SortableNumericAttribute)other;

            var result = this.NumericValue.CompareTo(otherSortableNumericAttribute.NumericValue);
            if (result == 0)
            {
                result = String.Compare(this.Value, otherSortableNumericAttribute.Value, StringComparison.Ordinal);
            }

            return result;
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

        public static bool operator ==(SortableNumericAttribute left, SortableNumericAttribute right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(SortableNumericAttribute left, SortableNumericAttribute right)
        {
            return !(left == right);
        }

        public static bool operator <(SortableNumericAttribute left, SortableNumericAttribute right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(SortableNumericAttribute left, SortableNumericAttribute right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(SortableNumericAttribute left, SortableNumericAttribute right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(SortableNumericAttribute left, SortableNumericAttribute right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }

#if DEBUG
        public override string ToString()
        {
            return $"D{this.NumericValue}";
        }
#endif
    }
}