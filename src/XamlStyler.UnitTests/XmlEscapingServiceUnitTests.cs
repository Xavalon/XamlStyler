// (c) Xavalon. All rights reserved.

using System.Xml.Linq;
using NUnit.Framework;
using Xavalon.XamlStyler.Services;

namespace Xavalon.XamlStyler.UnitTests
{
    [TestFixture]
    public class XmlEscapingServiceUnitTests
    {
        [Test]
        public void TestEscapeDocument()
        {
            var original = "";
            var expected = "";

            var xmlEscapingService = new XmlEscapingService();
            var actual = xmlEscapingService.EscapeDocument(original);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private string Issue426Xml = @"<ResourceDictionary 
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!--  Comment that contains 'xmlns:'  -->

    <Style x:Key=""SomeStyle"" />

</ResourceDictionary>";

        [Test]
        public void Issue426_CanParse()
        {
            CanParseOnceEscaped(Issue426Xml);
        }

        [Test]
        public void Issue426_CanEscapeAndUnescape()
        {
            var actual = EscapeAndUnescape(Issue426Xml);

            Assert.That(actual, Is.EqualTo(Issue426Xml));
        }

        private static void CanParseOnceEscaped(string unescapedXaml)
        {
            var xmlEscapingService = new XmlEscapingService();
            var escapedDocument = xmlEscapingService.EscapeDocument(unescapedXaml);

            var _ = XDocument.Parse(escapedDocument, LoadOptions.PreserveWhitespace);

            // No assert here as if this fails the above line will throw an exception
        }

        private static string EscapeAndUnescape(string unescapedXaml)
        {
            var xmlEscapingService = new XmlEscapingService();
            var escapedDocument = xmlEscapingService.EscapeDocument(unescapedXaml);
            var result = xmlEscapingService.UnescapeDocument(escapedDocument);

            return result;
        }
    }
}
