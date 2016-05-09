// © Xavalon. All rights reserved.

using NUnit.Framework;
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
            var indentService = new IndentService(true, 4);
            var result = indentService.Normalize(sourceText);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}