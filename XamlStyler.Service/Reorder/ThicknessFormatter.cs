using System.Text;
using System.Text.RegularExpressions;

namespace XamlStyler.Core.Reorder
{
    public static class ThicknessFormatter
    {
        private static readonly Regex[] Capture = new Regex[]
        {
            new Regex(@"^\s*(?<c1>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*$", RegexOptions.Compiled),
            new Regex(@"^\s*(?<c1>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*[ ,]\s*(?<c2>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*$", RegexOptions.Compiled),
            new Regex(@"^\s*(?<c1>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*[ ,]\s*(?<c2>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*[ ,]\s*(?<c3>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*[ ,]\s*(?<c4>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*$", RegexOptions.Compiled),
        };

        public static bool TryFormat(string s, char separator, out string formatted)
        {
            foreach (var regex in Capture)
            {
                var matches = regex.Matches(s);
                if (matches.Count == 1)
                {
                    formatted = Format(matches[0], separator);
                    return true;
                }
            }

            formatted = null;
            return false;
        }

        private static string Format(Match match, char separator)
        {
            var sb = new StringBuilder();
            foreach (Group g in match.Groups)
            {
                if (g.GetType() == typeof (Group))
                {
                    if (sb.Length > 0) sb.Append(separator);
                    sb.Append(g.Value);
                }
            }

            return sb.ToString();
        }
    }
}