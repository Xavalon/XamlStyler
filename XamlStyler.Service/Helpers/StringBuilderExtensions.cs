using System.Linq;
using System.Text;

namespace XamlStyler.Core.Helpers
{
    public static class StringBuilderExtensions
    {
        public static bool IsNewLine(this StringBuilder sb)
        {
            return sb.Length > 0 && sb[sb.Length - 1] == '\n';
        }

        /// <summary>
        /// Get index of last occurence of char
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int LastIndexOf(this StringBuilder sb, char value)
        {
            for (int i = sb.Length-1; i >= 0; i--)
            {
                if (sb[i] == value)
                    return i;
            }
            return -1;
        }

        public static string Substring(this StringBuilder sb, int startIndex, int length)
        {
            return sb.ToString(startIndex, length);
        }

        public static StringBuilder TrimEnd(this StringBuilder sb, params char[] trimChars)
        {
            int index = sb.Length;
            while (index > 0 && trimChars.Contains(sb[index-1]))
            {
                index--;
            }
            sb.Length = index;
            return sb;
        }

        /// <summary>
        /// Trim all trimchars from end of stringbuilder except if escaped
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="trimChars"></param>
        public static StringBuilder TrimUnescaped(this StringBuilder sb, params char[] trimChars)
        {
            int index = sb.Length;
            while (index > 0 && trimChars.Contains(sb[index-1]))
            {
                if (index > 1 && sb[index - 2] == '\\')
                    break;
                index--;
            }
            sb.Length = index;
            return sb;
        }
    }
}
