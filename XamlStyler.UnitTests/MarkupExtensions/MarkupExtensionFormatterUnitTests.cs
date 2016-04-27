using NUnit.Framework;
using Xavalon.XamlStyler.Core.Extensions;
using Xavalon.XamlStyler.Core.MarkupExtensions.Formatter;
using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.UnitTests.MarkupExtensions
{
    [TestFixture]
    public class MarkupExtensionFormatterUnitTests
    {
        private MarkupExtensionParser _parser;
        private MarkupExtensionFormatter _formatter;

        [SetUp]
        public void Setup()
        {
            _parser = new MarkupExtensionParser();
            _formatter = new MarkupExtensionFormatter(new[] { "x:Bind" });
        }

        [TestCase("{Hello}", "{Hello}")]
        [TestCase("{Hello world}", "{Hello world}")]
        [TestCase("{Hello big world}", "{Hello big world}")]
        [TestCase("{Hello big,world}", "{Hello big,\n       world}")]
        [TestCase("{Hello big=world}", "{Hello big=world}")]
        [TestCase("{The Answer,is=42}", "{The Answer,\n     is=42}")]
        [TestCase("{The Answer , is = 42}", "{The Answer,\n     is=42}")]
        [TestCase("{A {x:B c}}", "{A {x:B c}}")]
        [TestCase("{A {x:B c}, D={x:E f}}", "{A {x:B c},\n   D={x:E f}}")]
        [TestCase("{A B, C={D E,F={G H}}}", "{A B,\n   C={D E,\n        F={G H}}}")]
        [TestCase(
@"{Binding {}Title, RelativeSource={RelativeSource FindAncestor,AncestorType ={x:Type Page}},StringFormat={}{0}Now{{0}} - {0}}",
@"{Binding {}Title,
         RelativeSource={RelativeSource FindAncestor,
                                        AncestorType={x:Type Page}},
         StringFormat={}{0}Now{{0}} - {0}}"
)]
        public void TestFormatter(string sourceText, string expected)
        {
            MarkupExtension markupExtension;
            Assert.That(_parser.TryParse(sourceText, out markupExtension), Is.EqualTo(true));

            var result = _formatter.Format(markupExtension);
            Assert.That(result, Is.EqualTo(expected.GetLines()));
        }

        [TestCase("{Hello}", "{Hello}")]
        [TestCase("{Hello world}", "{Hello world}")]
        [TestCase("{Hello big world}", "{Hello big world}")]
        [TestCase("{Hello big,world}", "{Hello big, world}")]
        [TestCase("{Hello big=world}", "{Hello big=world}")]
        [TestCase("{The Answer,is=42}", "{The Answer, is=42}")]
        [TestCase("{The Answer , is = 42}", "{The Answer, is=42}")]
        [TestCase("{A {x:B c}}", "{A {x:B c}}")]
        [TestCase("{A {x:B c}, D={x:E f}}", "{A {x:B c}, D={x:E f}}")]
        [TestCase("{A B, C={D E,F={G H}}}", "{A B, C={D E, F={G H}}}")]
        public void TestSingleLineFormatter(string sourceText, string expected)
        {
            MarkupExtension markupExtension;
            Assert.That(_parser.TryParse(sourceText, out markupExtension), Is.EqualTo(true));

            var result = _formatter.FormatSingleLine(markupExtension);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}