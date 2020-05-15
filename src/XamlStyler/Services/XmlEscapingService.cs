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
            source = this.xmlnsAliasesBypassRegex.Replace(source, @"xmlns$1=""[${prefix}]${ns}""");

            return source;
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