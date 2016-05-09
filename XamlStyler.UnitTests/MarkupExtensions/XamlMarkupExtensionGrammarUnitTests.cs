// © Xavalon. All rights reserved.

using Irony.Parsing;
using NUnit.Framework;
using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;

namespace Xavalon.XamlStyler.UnitTests.MarkupExtensions
{
    [TestFixture]
    public class XamlMarkupExtensionGrammarUnitTests
    {
        private Parser parser;

        [SetUp]
        public void Setup()
        {
            var grammar = new XamlMarkupExtensionGrammar();
            var language = new LanguageData(grammar);

            this.parser = new Parser(language) { Context = { TracingEnabled = true } };
        }

        [TestCase("{Hello}")]
        [TestCase("{ Hello}")]
        [TestCase("{ Hello }")]
        [TestCase("{Hello world}")]
        [TestCase("{Hello big world}")]
        [TestCase("{Hello big, world}")]
        [TestCase("{Hello big=world}")]
        [TestCase("{The Answer,is=42}")]
        [TestCase("{The Answer , is = 42}")]
        [TestCase("{d:DesignInstance viewModels:ScenariosViewModel, IsDesignTimeCreatable = True}")]
        [TestCase("{Binding Command, RelativeSource={RelativeSource TemplatedParent}}")]
        [TestCase("{Binding ActualHeight, Mode=OneWay, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type gpc:MainWindow}}}")]
        [TestCase("{x:Type TextBox}")]
        [TestCase("{x:Null}")]
        [TestCase("{Binding DataContext.Orientation, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type questionViews:RadioButtonQuestionView}}}")]
        [TestCase("{Binding ErrorCount, Converter={StaticResource GreaterThanZeroToVisibilityConverter}}")]
        [TestCase("{Hello 'World'}")]
        [TestCase(@"{Hello 'Wor\'ld'}")]
        [TestCase(@"{Binding Title,
                              RelativeSource={RelativeSource FindAncestor,
                                                             AncestorType={x:Type Page}},
                              StringFormat={}{0}Now{{0}} - {0}}")]
        [TestCase(@"{Binding Path=Content,
                              ElementName=m_button,
                              StringFormat={}{0:##\,#0.00;(##\,#0.00); }}")]
        // Odd case, but is valid according to specifications. Result in TYPENAME={
        [TestCase("{{}")]
        public void TestParserParsed(string sourceText)
        {
            ParseTree tree = this.parser.Parse(sourceText);

            Assert.That(tree, Is.Not.Null);
            Assert.That(tree.Status, Is.EqualTo(ParseTreeStatus.Parsed));
        }

        [TestCase("")]
        [TestCase("{")]
        [TestCase("{}")]
        [TestCase("{}}")]
        [TestCase("{{}}")]
        [TestCase("{Hello}}")]
        public void TestParserError(string sourceText)
        {
            ParseTree tree = this.parser.Parse(sourceText);

            Assert.That(tree, Is.Not.Null);
            Assert.That(tree.Status, Is.EqualTo(ParseTreeStatus.Error));
        }
    }
}