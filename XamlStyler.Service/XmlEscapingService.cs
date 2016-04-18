using System.Text.RegularExpressions;

namespace XamlStyler.Core
{
    public class XmlEscapingService
    {
        private readonly Regex _htmlReservedCharRegex = new Regex(@"&([\d\D][^;]{3,7});");
        private readonly Regex _htmlReservedCharRestoreRegex = new Regex(@"__amp__([^;]{2,7})__scln__");

        protected string EscapeDocument(string source)
        {
            return _htmlReservedCharRegex.Replace(source, @"__amp__$1__scln__");
        }

        protected string UnescapeDocument(string source)
        {
            return _htmlReservedCharRestoreRegex.Replace(source, @"&$1;");
        }
    }
}