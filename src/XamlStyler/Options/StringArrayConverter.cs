// (c) Xavalon. All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xavalon.XamlStyler.Extensions;

namespace Xavalon.XamlStyler.Options
{
    public class StringArrayConverter : ArrayConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;

            return !String.IsNullOrEmpty(stringValue)
                ? stringValue.GetLines().ToArray()
                : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, 
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            var stringArray = value as string[];

            return (stringArray != null)
                ? String.Join(Environment.NewLine, stringArray)
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}