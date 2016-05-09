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

#if DEBUG

        public override string ToString()
        {
            return "D" + this.NumericValue;
        }

#endif
    }
}