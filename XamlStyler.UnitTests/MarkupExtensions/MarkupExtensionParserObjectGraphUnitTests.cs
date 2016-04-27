using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.UnitTests.MarkupExtensions
{
    [TestFixture]
    public class MarkupExtensionParserObjectGraphUnitTests
    {
        private void ParseAndAssertObjectGraph(string sourceText, MarkupExtension expected)
        {
            IMarkupExtensionParser markupExtensionParser = new MarkupExtensionParser();
            MarkupExtension actual;
            var result = markupExtensionParser.TryParse(sourceText, out actual);

            Assert.That(result, Is.True);
            Assert.That(actual, Is.Not.Null);

            var compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(expected, actual);
            Assert.That(compareResult.AreEqual, Is.True, compareResult.DifferencesString);
        }

        [Test]
        public void TestHello()
        {
            var sourceText = "{Hello}";
            var expected = new MarkupExtension("Hello");

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        [Test]
        public void TestBindingHello()
        {
            var sourceText = "{Binding Hello}";
            var expected = new MarkupExtension("Binding",
                new PositionalArgument("Hello")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        [Test]
        public void TestHelloWorld42()
        {
            var sourceText = "{Hello world=42}";
            var expected = new MarkupExtension("Hello",
                new NamedArgument("world", "42")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        [Test]
        public void TestBindingHelloFooBar()
        {
            var sourceText = "{Binding Hello, foo,bar}";
            var expected = new MarkupExtension("Binding",
                new PositionalArgument("Hello"),
                new PositionalArgument("foo"),
                new PositionalArgument("bar")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        [Test]
        public void TestHelloworldFoo1Bar42()
        {
            var sourceText = "{Hello world,foo=1,bar=42}";
            var expected = new MarkupExtension("Hello",
                new PositionalArgument("world"),
                new NamedArgument("foo","1"),
                new NamedArgument("bar","42")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        [Test]
        public void TestHelloworldFoo1Bar42_spaced()
        {
            var sourceText = "  {  Hello   world ,  foo  =  1  ,  bar = 42  }  ";
            var expected = new MarkupExtension("Hello",
                new PositionalArgument("world"),
                new NamedArgument("foo","1"),
                new NamedArgument("bar","42")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }
        [Test]
        public void TestSignificantSpacing()
        {
            var sourceText = @"  { Binding  StringFormat = {} {0}\  , Mode = OneWay } ";
            var expected = new MarkupExtension("Binding",
                new NamedArgument("StringFormat", @"{} {0}\ "),
                new NamedArgument("Mode","OneWay")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        [Test]
        public void TestNestedBinding()
        {
            var sourceText = "{Binding ActualHeight, Mode=OneWay, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type gpc:MainWindow}}}";
            var expected = new MarkupExtension("Binding",
                new PositionalArgument("ActualHeight"),
                new NamedArgument("Mode", "OneWay"),
                new NamedArgument("RelativeSource", new MarkupExtension("RelativeSource",
                    new PositionalArgument("FindAncestor"),
                    new NamedArgument("AncestorType",
                        new MarkupExtension("x:Type", new PositionalArgument("gpc:MainWindow")))))
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        [Test]
        public void TestNestedBinding2()
        {
            var sourceText = @"{DynamicResource {x:Static SystemColors.ControlTextBrushKey},
                                       ResourceKey={x:Static SystemColors.ControlTextBrushKey}}";
            var expected = new MarkupExtension("DynamicResource",
                new PositionalArgument(new MarkupExtension("x:Static", new PositionalArgument("SystemColors.ControlTextBrushKey"))),
                new NamedArgument("ResourceKey", new MarkupExtension("x:Static", new PositionalArgument("SystemColors.ControlTextBrushKey")))
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        [Test]
        public void TestNestedBindingWithBraceFormat()
        {
            var sourceText = @"{Binding Title,
                              RelativeSource={RelativeSource FindAncestor,
                                                             AncestorType={x:Type UserControl}},
                              StringFormat={}{0}Now{{0}} - {0}}";
            var expected = new MarkupExtension("Binding",
                new PositionalArgument("Title"),
                new NamedArgument("RelativeSource",
                    new MarkupExtension("RelativeSource",
                        new PositionalArgument("FindAncestor"),
                        new NamedArgument("AncestorType",
                            new MarkupExtension("x:Type", new PositionalArgument("UserControl"))))),
                new NamedArgument("StringFormat", "{}{0}Now{{0}} - {0}")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        /// <summary>
        /// Test issue #94. Slightly invalid markup
        /// </summary>
        [Test]
        public void TestEscapedBraces()
        {
            var sourceText = @"{Binding Value, StringFormat=\{0:N2\}}";
            var expected = new MarkupExtension("Binding",
                new PositionalArgument("Value"),
                new NamedArgument("StringFormat", @"\{0:N2\}")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }

        /// <summary>
        /// Test issue #94 valid markup
        /// </summary>
        [Test]
        public void TestEscapedBraces2()
        {
            var sourceText = @"{Binding Value, StringFormat={}{0:N2}}";
            var expected = new MarkupExtension("Binding",
                new PositionalArgument("Value"),
                new NamedArgument("StringFormat", "{}{0:N2}")
                );

            ParseAndAssertObjectGraph(sourceText, expected);
        }
    }
}