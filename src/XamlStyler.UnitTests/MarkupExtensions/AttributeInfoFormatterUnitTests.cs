// (c) Xavalon. All rights reserved.

using System;
using NUnit.Framework;
using Xavalon.XamlStyler.MarkupExtensions.Formatter;
using Xavalon.XamlStyler.Model;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Services;

namespace Xavalon.XamlStyler.UnitTests.MarkupExtensions
{
    [TestFixture]
    public class AttributeInfoFormatterUnitTests
    {
        private AttributeInfoFormatter formatter;

        [SetUp]
        public void Setup()
        {
            var options = new StylerOptions
            {
                IndentSize = 4,
                IndentWithTabs = false,
                AttributeIndentationStyle = AttributeIndentationStyle.Spaces,
            };
            var indentService = new IndentService(options);
            var markupExtensionFormatter = new MarkupExtensionFormatter(new[] { "x:Bind", "Binding" });
            this.formatter = new AttributeInfoFormatter(markupExtensionFormatter, indentService);
        }

        private static AttributeInfo MakePlainAttr(string name, string value)
        {
            return new AttributeInfo(
                name,
                value,
                attributeHasIgnoredNamespace: false,
                attributeNameWithoutNamespace: name,
                orderRule: new AttributeOrderRule("*", 0, 0),
                markupExtension: null);
        }

        /// <summary>
        /// ToCommaDelimitedMultiLineString should place each comma-separated segment on its own line,
        /// aligned to the first character of the value (i.e. right after the opening quote).
        /// </summary>
        [TestCase(
            "Selector",
            "ContentControl Button, ContentControl StackPanel",
            "    ",
            "Selector=\"ContentControl Button,\n              ContentControl StackPanel\"")]
        [TestCase(
            "Text",
            "Alpha, Beta, Gamma",
            "    ",
            "Text=\"Alpha,\n          Beta,\n          Gamma\"")]
        [TestCase(
            "Selector",
            "ContentControl Button",
            "    ",
            "Selector=\"ContentControl Button\"")]
        public void TestCommaDelimitedMultiLine(string name, string value, string baseIndent, string expected)
        {
            var attrInfo = MakePlainAttr(name, value);
            var xamlLanguageOptions = new XamlLanguageOptions { IsFormatable = true };

            var result = this.formatter.ToCommaDelimitedMultiLineString(attrInfo, baseIndent, xamlLanguageOptions);

            // Normalize \n in expected to platform line ending so tests pass on both Windows and Unix
            Assert.That(result, Is.EqualTo(expected.Replace("\n", Environment.NewLine)));
        }
    }
}
