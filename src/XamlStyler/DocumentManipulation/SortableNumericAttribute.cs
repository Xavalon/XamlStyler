// (c) Xavalon. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Xavalon.XamlStyler.DocumentManipulation
{
    // TODO: Fully implement IComparable interface.
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "No clear ROI and introduces more warnings")]
    public class SortableNumericAttribute : ISortableAttribute
    {
        public string Value { get; private set; }

        public double NumericValue { get; private set; }

        public SortableNumericAttribute(string value, double defaultNumericValue)
        {
            this.Value = value;

            this.NumericValue = Double.TryParse(value, out double numericValue)
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

#if DEBUG
        public override string ToString()
        {
            return $"D{this.NumericValue}";
        }
#endif
    }
}