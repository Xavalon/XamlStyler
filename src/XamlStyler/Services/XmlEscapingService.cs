// (c) Xavalon. All rights reserved.

using System.Text.RegularExpressions;

namespace Xavalon.XamlStyler.Services
{
    public class XmlEscapingService
    {
        private readonly Regex htmlReservedCharRegex = new Regex(@"&([\d\D][^;]{1,7});");
        private readonly Regex htmlReservedCharRestoreRegex = new Regex(@"__amp__([\d\D][^;]{1,7})__scln__");
        private readonly Regex xmlnsAliasesBypassRegex = new Regex(@"xmlns(:(?<prefix>[^=]+))=""(?<ns>[^""]+)""");
        private readonly Regex xmlnsAliasesBypassRestoreRegex = new Regex(@"xmlns:(?<prefix>[^=]+)=""\[\1\](?<ns>[^""]+)""");

        public string EscapeDocument(string source)
        {
            source = this.htmlReservedCharRegex.Replace(source, @"__amp__$1__scln__");
            source = this.xmlnsAliasesBypassRegex.Replace(source, evaluator: SelectiveXmlReplacer);

            return source;
        }

        private string SelectiveXmlReplacer(Match match)
        {
            // This check allows for partial xmlns definitions in a comment to not break anything.
            // See https://github.com/Xavalon/XamlStyler/issues/426
            if (match.Captures.Count == 1 && match.Captures[0].Value.Contains("-->"))
            {
                return match.Captures[0].Value;
            }
            else
            {
                return this.xmlnsAliasesBypassRegex.Replace(match.Value, @"xmlns$1=""[${prefix}]${ns}""");
            }
        }

        public string UnescapeDocument(string source)
        {
            source = this.htmlReservedCharRestoreRegex.Replace(source, @"&$1;");
            source = this.xmlnsAliasesBypassRestoreRegex.Replace(source, @"xmlns:${prefix}=""${ns}""");

            return source;
        }

        internal string RestoreXmlnsAliasesBypass(string source)
        {
            return this.xmlnsAliasesBypassRestoreRegex.Replace(source, @"xmlns:${prefix}=""${ns}""");
        }
    }
}