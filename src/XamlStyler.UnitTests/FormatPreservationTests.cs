// (c) Xavalon. All rights reserved.

using System;
using NUnit.Framework;
using Xavalon.XamlStyler;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.UnitTests
{
    [TestFixture]
    public class FormatPreservationTests
    {
        [Test]
        public void KeepOriginalElementFormatPreservesSelfClosingAndEndTags()
        {
            var options = new StylerOptions
            {
                KeepOriginalElementFormat = true,
                RemoveEndingTagOfEmptyElement = false,
                IndentSize = 2,
                IndentWithTabs = false
            };

            var stylerService = new StylerService(options, new XamlLanguageOptions { IsFormatable = true });

            var input = string.Join(Environment.NewLine,
                "<Root>",
                "  <Button />",
                "  <TextBlock></TextBlock>",
                "</Root>");

            var expected = string.Join(Environment.NewLine,
                "<Root>",
                "  <Button />",
                "  <TextBlock></TextBlock>",
                "</Root>");

            Assert.That(stylerService.StyleDocument(input), Is.EqualTo(expected));
        }

        [Test]
        public void KeepOriginalAttributeLineBreaksPreservesLayoutAndOrder()
        {
            var options = new StylerOptions
            {
                KeepOriginalAttributeLineBreaks = true,
                EnableAttributeReordering = true,
                IndentSize = 2,
                IndentWithTabs = false
            };

            var stylerService = new StylerService(options, new XamlLanguageOptions { IsFormatable = true });

            var input = string.Join(Environment.NewLine,
                "<Root>",
                "  <Button Width=\"20\"",
                "          Height=\"10\" Margin=\"1\"",
                "          Padding=\"2\" />",
                "</Root>");

            var expected = string.Join(Environment.NewLine,
                "<Root>",
                "  <Button Width=\"20\"",
                "    Height=\"10\" Margin=\"1\"",
                "    Padding=\"2\" />",
                "</Root>");

            Assert.That(stylerService.StyleDocument(input), Is.EqualTo(expected));
        }
    }
}
