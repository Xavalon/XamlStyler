using System;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class SortableNumericAttribute : ISortableAttribute
    {
        public string Value { get; private set; }

        public double NumericValue { get; private set; }

        public SortableNumericAttribute(string value, double defaultNumericValue)
        {
            Value = value;

            double numericValue;
            NumericValue = Double.TryParse(value, out numericValue)
                ? numericValue
                : defaultNumericValue;
        }

        public int CompareTo(ISortableAttribute other)
        {
            var otherSortableNumericAttribute = (SortableNumericAttribute)other;

            var result = NumericValue.CompareTo(otherSortableNumericAttribute.NumericValue);
            if (result == 0)
                result = String.Compare(Value, otherSortableNumericAttribute.Value, StringComparison.Ordinal);
            return result;
        }

#if DEBUG

        public override string ToString()
        {
            return "D" + NumericValue;
        }

#endif
    }
}