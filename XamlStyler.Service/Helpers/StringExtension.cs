using System.Text;

namespace XamlStyler.Core.Helpers
{
    public static class StringExtension
    {
        public static string ToXmlEncodedString(this string input, bool ignoreCarrier = false)
        {
            var buffer = new StringBuilder(input);

            buffer.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");

            if (!ignoreCarrier)
            {
                buffer.Replace("\n", "&#10;");
            }

            return buffer.ToString();
        }
    }
}