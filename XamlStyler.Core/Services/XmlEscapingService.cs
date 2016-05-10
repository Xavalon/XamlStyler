// © Xavalon. All rights reserved.

using System.Text.RegularExpressions;

namespace Xavalon.XamlStyler.Core.Services
{
    public class XmlEscapingService
    {
        private readonly Regex htmlReservedCharRegex = new Regex(@"&([\d\D][^;]{3,7});");
        private readonly Regex htmlReservedCharRestoreRegex = new Regex(@"__amp__([^;]{2,7})__scln__");

        public string EscapeDocument(string source)
        {
            return this.htmlReservedCharRegex.Replace(source, @"__amp__$1__scln__");
        }

        public string UnescapeDocument(string source)
        {
            return this.htmlReservedCharRestoreRegex.Replace(source, @"&$1;");
        }
    }
}