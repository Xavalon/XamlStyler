// (c) Xavalon. All rights reserved.

using System.Linq;
using System.Text;

namespace Xavalon.XamlStyler.Extensions
{
    public static class StringBuilderExtensions
    {
        public static bool IsNewLine(this StringBuilder stringBuilder)
        {
            return (stringBuilder.Length > 0)
                && (stringBuilder[stringBuilder.Length - 1] == '\n');
        }

        /// <summary>
        /// Get index of last occurrence of char
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int LastIndexOf(this StringBuilder stringBuilder, char value)
        {
            for (int i = stringBuilder.Length - 1; i >= 0; i--)
            {
                if (stringBuilder[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }

        public static string Substring(this StringBuilder stringBuilder, int startIndex, int length)
        {
            return stringBuilder.ToString(startIndex, length);
        }

        public static StringBuilder TrimEnd(this StringBuilder stringBuilder, params char[] trimChars)
        {
            int index = stringBuilder.Length;
            while ((index > 0) && trimChars.Contains(stringBuilder[index - 1]))
            {
                index--;
            }

            stringBuilder.Length = index;
            return stringBuilder;
        }

        /// <summary>
        /// Trim all trimchars from end of stringBuilder except if escaped
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="trimChars"></param>
        public static StringBuilder TrimUnescaped(this StringBuilder stringBuilder, params char[] trimChars)
        {
            int index = stringBuilder.Length;
            while ((index > 0) && trimChars.Contains(stringBuilder[index - 1]))
            {
                if ((index > 1) && (stringBuilder[index - 2] == '\\'))
                {
                    break;
                }

                index--;
            }

            stringBuilder.Length = index;
            return stringBuilder;
        }
    }
}