using NUnit.Framework;
using Xavalon.XamlStyler.Core.Options;
using Xavalon.XamlStyler.Core.Services;

namespace Xavalon.XamlStyler.UnitTests
{
    [TestFixture]
    public class IndentServiceUnitTests
    {
        [TestCase("   ", "   ")]
        [TestCase("    ", "\t")]
        [TestCase("     ", "\t ")]
        [TestCase("\t", "\t")]
        [TestCase("\t ", "\t ")]
        [TestCase("\t  ", "\t  ")]
        [TestCase("\t   ", "\t   ")]
        [TestCase("\t    ", "\t\t")]
        [TestCase("\t \t    ", "\t \t    ")]
        [TestCase("\t Hello    ", "\t Hello    ")]
        [TestCase("         Hi", "\t\t Hi")]
        public void TestNormalize(string sourceText, string expected)
        {
            var indentService = new IndentService(new StylerOptions()
            {
                IndentWithTabs = true,
                IndentSize = 4,
                AttributeIndentationStyle = AttributeIndentationStyle.Mixed
            });
                
            var result = indentService.Normalize(sourceText);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("   ")]
        [TestCase("    ")]
        [TestCase("     ")]
        [TestCase("\t")]
        [TestCase("\t ")]
        [TestCase("\t  ")]
        [TestCase("\t   ")]
        [TestCase("\t    ")]
        [TestCase("\t \t    ")]
        [TestCase("\t Hello    ")]
        [TestCase("         Hi")]
        public void TestNormalizeNoOp(string sourceText)
        {
            var indentService = new IndentService(new StylerOptions()
            {
                IndentWithTabs = true,
                IndentSize = 4,
                AttributeIndentationStyle = AttributeIndentationStyle.Spaces
            });
                
            var result = indentService.Normalize(sourceText);
            Assert.That(result, Is.EqualTo(sourceText));

            indentService = new IndentService(new StylerOptions()
            {
                IndentWithTabs = false,
                IndentSize = 4,
                AttributeIndentationStyle = AttributeIndentationStyle.Mixed
            });
                
            result = indentService.Normalize(sourceText);
            Assert.That(result, Is.EqualTo(sourceText));
        }
    }
}
