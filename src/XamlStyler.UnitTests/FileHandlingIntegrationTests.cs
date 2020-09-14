// (c) Xavalon. All rights reserved.

using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xavalon.XamlStyler.DocumentManipulation;
using Xavalon.XamlStyler.Options;

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
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributeIndentation = attributeIndentation,
                AttributesTolerance = 0,
                MaxAttributeCharactersPerLine = 80,
                MaxAttributesPerLine = 3,
                PutEndingBracketOnNewLine = true
            };

            this.DoTestCase(stylerOptions, attributeIndentation);
        }

        [Test]
        public void TestDesignReferenceRemoval()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                RemoveDesignTimeReferences = true
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeThresholdHandling()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributesTolerance = 0,
                MaxAttributeCharactersPerLine = 80,
                MaxAttributesPerLine = 3,
                PutEndingBracketOnNewLine = true
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeToleranceHandling()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                AttributesTolerance = 3,
                RootElementLineBreakRule = LineBreakRule.Always,
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [TestCase(0)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TestCommentHandling(byte testNumber)
        {
            var stylerOptions = new StylerOptions
            {
                IndentSize = 2,
                CommentSpaces = testNumber,
            };

            this.DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestCommentAtFirstLine()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestDefaultHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestSuppressedDefaultHandling()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                SuppressProcessing = true
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeSortingOptionHandling()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
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

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestxBindSplitting()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                NoNewLineMarkupExtensions = "x:Bind"
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestBindingSplitting()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                NoNewLineMarkupExtensions = "x:Bind, Binding"
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [TestCase(false, 2)]
        [TestCase(false, 4)]
        [TestCase(true, 2)]
        [TestCase(true, 4)]
        public void TestMarkupExtensionHandling(bool indentWithTabs, int tabSize)
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                FormatMarkupExtension = true,
                IndentWithTabs = indentWithTabs,
                IndentSize = tabSize,
                AttributeIndentationStyle = AttributeIndentationStyle.Mixed,
            };

            this.DoTestCase(stylerOptions, $"{tabSize}_{(indentWithTabs ? "tabs" : "spaces")}");
        }

        [Test]
        public void TestNestedCustomMarkupExtensionsWithBindings()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                KeepFirstAttributeOnSameLine = false,
                AttributesTolerance = 1,
                NoNewLineMarkupExtensions = "x:Bind, Binding"
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestSingleLineNestedMarkupExtensions()
        {
            FileHandlingIntegrationTests.DoTest(new StylerOptions
            {
                IndentSize = 2
            });
        }

        [Test]
        public void TestMarkupWithAttributeNotOnFirstLine()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                KeepFirstAttributeOnSameLine = false,
                AttributesTolerance = 1
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestNoContentElementHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestTextOnlyContentElementHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestGridChildrenHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestNestedGridChildrenHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestCanvasChildrenHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestNestedCanvasChildrenHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestNestedPropertiesAndChildrenHandling()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderVSM = VisualStateManagerRule.First
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestKeepSelectAttributesOnFirstLine()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                FirstLineAttributes = "x:Name, x:Key"
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeOrderRuleGroupsOnSeparateLinesHandling()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                MaxAttributesPerLine = 3,
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestProcessingInstructionHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestXmlnsAliasesHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [TestCase(ReorderSettersBy.Property)]
        [TestCase(ReorderSettersBy.TargetName)]
        [TestCase(ReorderSettersBy.TargetNameThenProperty)]
        public void TestReorderSetterHandling(ReorderSettersBy reorderSettersBy)
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
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
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                SpaceBeforeClosingSlash = spaceBeforeClosingSlash
            };

            this.DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestCDATAHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestXmlSpaceHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [TestCase(ThicknessStyle.None)]
        [TestCase(ThicknessStyle.Comma)]
        [TestCase(ThicknessStyle.Space)]
        public void TestThicknessHandling(ThicknessStyle thicknessStyle)
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
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
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
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
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestWildCard()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
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

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestValueXmlEntityHandling()
        {
            FileHandlingIntegrationTests.DoTest(this.GetLegacyStylerOptions());
        }

        [Test]
        public void TestVisualStateManagerNone()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderVSM = VisualStateManagerRule.None
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestVisualStateManagerFirst()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderVSM = VisualStateManagerRule.First
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestVisualStateManagerLast()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"))
            {
                ReorderVSM = VisualStateManagerRule.Last
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeIgnoreEncode()
        {
            var stylerOptions = new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\TestAttributeIgnoreEncode.json"))
            {
                IgnoreEncodeForAttributes = "Selector"
            };

            FileHandlingIntegrationTests.DoTest(stylerOptions);
        }

        private static void DoTest(
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

            var activeDir = Path.GetDirectoryName(new Uri(typeof(FileHandlingIntegrationTests).Assembly.CodeBase).LocalPath);
            var testFile = Path.Combine(activeDir, testFileBaseName);

            var testFileResultBaseName = (expectedSuffix != null)
                ? $"{testFile}_{expectedSuffix}"
                : testFile;

            // Exercise stylerService using supplied test XAML data
            string actualOutput = stylerService.StyleDocument(File.ReadAllText($"{testFile}.testxaml"));

            // Write output to ".actual" file for further investigation
            File.WriteAllText($"{testFileResultBaseName}.actual", actualOutput, Encoding.UTF8);

            // Check result
            Assert.That(actualOutput, Is.EqualTo(File.ReadAllText($"{testFileResultBaseName}.expected")));
        }

        private static string GetConfiguration(string path)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        }

        private StylerOptions GetLegacyStylerOptions()
        {
            return new StylerOptions(
                config: FileHandlingIntegrationTests.GetConfiguration(@"TestConfigurations\LegacyTestSettings.json"));
        }
    }
}