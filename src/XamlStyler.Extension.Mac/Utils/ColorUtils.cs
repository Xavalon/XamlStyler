// © Xavalon. All rights reserved.

using Gdk;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace Xavalon.XamlStyler.Extension.Mac.Utils
{
    public static class ColorUtils
    {
        private static readonly Dictionary<string, Color> _parsedColorCache = new Dictionary<string, Color>();

        public static Color Parse(string colorHex)
        {
            var fixedColorHex = colorHex.TrimStart('#');
            if (fixedColorHex.Length != 6)
            {
                throw new NotSupportedException("Should be exactly 6 hexademical digits for color: RRGGBB");
            }

            if (!_parsedColorCache.TryGetValue(fixedColorHex, out var color))
            {
                var red = byte.Parse(fixedColorHex.Substring(0, 2), NumberStyles.HexNumber);
                var green = byte.Parse(fixedColorHex.Substring(2, 2), NumberStyles.HexNumber);
                var blue = byte.Parse(fixedColorHex.Substring(4, 2), NumberStyles.HexNumber);

                color = new Color(red, green, blue);
                _parsedColorCache[fixedColorHex] = color;
            }

            return color;
        }
    }
}