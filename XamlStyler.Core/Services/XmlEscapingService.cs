using System.Text.RegularExpressions;

namespace Xavalon.XamlStyler.Core.Services
{
    public class XmlEscapingService
    {
        private readonly Regex _htmlReservedCharRegex = new Regex(@"&([\d\D][^;]{1,7});");
        private readonly Regex _htmlReservedCharRestoreRegex = new Regex(@"__amp__([\d\D][^;]{1,7})__scln__");

        public string EscapeDocument(string source)
        {
            return _htmlReservedCharRegex.Replace(source, @"__amp__$1__scln__");
        }

        public string UnescapeDocument(string source)
        {
            return _htmlReservedCharRestoreRegex.Replace(source, @"&$1;");
        }
    }
}