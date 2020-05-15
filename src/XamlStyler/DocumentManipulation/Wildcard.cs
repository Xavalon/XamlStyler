// (c) Xavalon. All rights reserved.

using System.Text.RegularExpressions;

namespace Xavalon.XamlStyler.DocumentManipulation
{
    /// <summary>
    /// Create regular expression (RegEx) from simple "DOS" wildcard (* and ?).
    /// </summary>
    public class Wildcard : Regex
    {
        public Wildcard(string pattern)
            : base(WildcardToRegex(pattern))
        {
        }

        public Wildcard(string pattern, RegexOptions options)
            : base(WildcardToRegex(pattern), options)
        {
        }

        public static string WildcardToRegex(string pattern)
        {
            return "^" + Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }
    }
}