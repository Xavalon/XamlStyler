using System.Text.RegularExpressions;

namespace XamlStyler.Core.Reorder
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
            return "^" + Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";
        }
    }
}