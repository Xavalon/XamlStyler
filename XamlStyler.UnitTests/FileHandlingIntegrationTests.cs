// © Xavalon. All rights reserved.

using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Text;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.DocumentManipulation;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.UnitTests
{
    [TestFixture]
    public class FileHandlingIntegrationTests
    {
        [TestCase(0)]
        [TestCase(4)]
        public void TestAttributeIndentationHandling(byte attributeIndentation)
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributeIndentation = attributeIndentation,
                AttributesTolerance = 0,
                MaxAttributeCharatersPerLine = 80,
                MaxAttributesPerLine = 3,
                PutEndingBracketOnNewLine = true
            };

            this.DoTestCase(stylerOptions, attributeIndentation);
        }

        [Test]
        public void TestDesignReferenceRemoval()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                RemoveDesignTimeReferences = true
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeThresholdHandling()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributesTolerance = 0,
                MaxAttributeCharatersPerLine = 80,
                MaxAttributesPerLine = 3,
                PutEndingBracketOnNewLine = true
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeToleranceHandling()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributesTolerance = 3,
                RootElementLineBreakRule = LineBreakRule.Always,
            };

            this.DoTest(stylerOptions);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TestCommentHandling(byte testNumber)
        {
            var stylerOptions = new StylerOptions
            {
                CommentSpaces = testNumber,
            };

            this.DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestCommentAtFirstLine()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestDefaultHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestAttributeSortingOptionHandling()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributeOrderingRuleGroups = new[]
                {
                    // Class definition group
                    "x:Class",
                    // WPF Namespaces group
                    "xmlns, xmlns:x",
                    // Other namespace
                    "xmlns:*",
                    // Element key group
                    "Key, x:Key, Uid, x:Uid",
                    // Element name group
                    "Name, x:Name, Title",
                    // Attached layout group
                    "Grid.Column, Grid.ColumnSpan, Grid.Row, Grid.RowSpan, Canvas.Right, Canvas.Bottom, Canvas.Left, Canvas.Top",
                    // Core layout group
                    "MinWidth, MinHeight, Width, Height, MaxWidth, MaxHeight, Margin",
                    // Alignment layout group
                    "Panel.ZIndex, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment",
                    // Unmatched
                    "*:*, *",
                    // Miscellaneous/Other attributes group
                    "Offset, Color, TargetName, Property, Value, StartPoint, EndPoint, PageSource, PageIndex",
                    // Blend related group
                    "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText",
                }
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestxBindSplitting()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestBindingSplitting()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                NoNewLineMarkupExtensions = "x:Bind, Binding"
            };

            this.DoTest(stylerOptions);
        }

        [TestCase(false, 2)]
        [TestCase(false, 4)]
        [TestCase(true, 2)]
        [TestCase(true, 4)]
        public void TestMarkupExtensionHandling(bool indentWithTabs, int tabSize)
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                FormatMarkupExtension = true,
                IndentWithTabs = indentWithTabs,
                IndentSize = tabSize,
                AttributeIndentationStyle = AttributeIndentationStyle.Mixed,
            };

            this.DoTestCase(stylerOptions, $"{tabSize}_{(indentWithTabs ? "tabs" : "spaces")}");
        }

        [Test]
        public void TestMarkupWithAttributeNotOnFirstLine()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                KeepFirstAttributeOnSameLine = false,
                AttributesTolerance = 1
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestNoContentElementHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestTextOnlyContentElementHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestGridChildrenHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestNestedGridChildrenHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestCanvasChildrenHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestNestedCanvasChildrenHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestNestedPropertiesAndChildrenHandling()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderVSM = VisualStateManagerRule.First
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestKeepSelectAttributesOnFirstLine()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                FirstLineAttributes = "x:Name, x:Key"
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeOrderRuleGroupsOnSeparateLinesHandling()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                MaxAttributesPerLine = 3,
            };

            this.DoTest(stylerOptions);
        }

        [TestCase(ReorderSettersBy.Property)]
        [TestCase(ReorderSettersBy.TargetName)]
        [TestCase(ReorderSettersBy.TargetNameThenProperty)]
        public void TestReorderSetterHandling(ReorderSettersBy reorderSettersBy)
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderSetters = reorderSettersBy,
            };

            this.DoTestCase(stylerOptions, reorderSettersBy);
        }

        [TestCase(1, true)]
        [TestCase(2, false)]
        public void TestClosingElementHandling(int testNumber, bool spaceBeforeClosingSlash)
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                SpaceBeforeClosingSlash = spaceBeforeClosingSlash
            };

            this.DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestCDATAHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestXmlSpaceHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [TestCase(ThicknessStyle.None)]
        [TestCase(ThicknessStyle.Comma)]
        [TestCase(ThicknessStyle.Space)]
        public void TestThicknessHandling(ThicknessStyle thicknessStyle)
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ThicknessStyle = thicknessStyle
            };

            this.DoTestCase(stylerOptions, thicknessStyle);
        }

        [TestCase(1, LineBreakRule.Default)]
        [TestCase(2, LineBreakRule.Always)]
        [TestCase(3, LineBreakRule.Never)]
        public void TestRootHandling(int testNumber, LineBreakRule lineBreakRule)
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributesTolerance = 3,
                MaxAttributesPerLine = 4,
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                RootElementLineBreakRule = lineBreakRule,
            };

            this.DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestRunHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestWildCard()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributeOrderingRuleGroups = new[]
                {
                    "x:Class*",
                    "xmlns, xmlns:x",
                    "xmlns:*",
                    "Grid.*, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
                    "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight",
                    "*:*, *",
                    "ToolTipService.*, AutomationProperties.*",
                    "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText"
                }
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestValueXmlEntityHandling()
        {
            this.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestVisualStateManagerNone()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderVSM = VisualStateManagerRule.None
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestVisualStateManagerFirst()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderVSM = VisualStateManagerRule.First
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestVisualStateManagerLast()
        {
            var stylerOptions = new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderVSM = VisualStateManagerRule.Last
            };

            this.DoTest(stylerOptions);
        }

        private void DoTest([System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            this.DoTest(new StylerOptions(), callerMemberName);
        }

        private void DoTest(
            StylerOptions stylerOptions,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            FileHandlingIntegrationTests.DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), null);
        }

        private void DoTestCase<T>(
            StylerOptions stylerOptions,
            T testIdentifier,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            FileHandlingIntegrationTests.DoTest(
                stylerOptions, 
                Path.Combine("TestFiles", callerMemberName),
                testIdentifier.ToString());
        }

        /// <summary>
        /// Style input document and verify output against expected
        /// </summary>
        /// <param name="stylerOptions"></param>
        /// <param name="testFileBaseName"></param>
        /// <param name="expectedSuffix"></param>
        private static void DoTest(StylerOptions stylerOptions, string testFileBaseName, string expectedSuffix)
        {
            var stylerService = new StylerService(stylerOptions);

            var testFileResultBaseName = (expectedSuffix != null)
                ? $"{testFileBaseName}_{expectedSuffix}"
                : testFileBaseName;

            // Exercise stylerService using supplied test XAML data
            string actualOutput = stylerService.StyleDocument(File.ReadAllText($"{testFileBaseName}.testxaml"));

            // Write output to ".actual" file for further investigation
            File.WriteAllText($"{testFileResultBaseName}.actual", actualOutput, Encoding.UTF8);

            // Check result
            Assert.That(actualOutput, Is.EqualTo(File.ReadAllText($"{testFileResultBaseName}.expected")));
        }

        private string GetConfiguration(string path)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        }

        private StylerOptions GetLegacyStylerOptions()
        {
            return new StylerOptions(
                config: this.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"));
        }
    }
}