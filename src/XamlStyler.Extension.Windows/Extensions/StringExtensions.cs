// © Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Extension.Windows.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string self)
        {
            return String.IsNullOrEmpty(self);
        }

        public static bool IsNullOrWhiteSpace(this string self)
        {
            return String.IsNullOrWhiteSpace(self);
        }
    }
}
