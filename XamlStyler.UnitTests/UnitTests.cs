using System.Text;
using System.IO;
using NUnit.Framework;
using XamlStyler.Core;
using XamlStyler.Core.Options;

namespace XamlStyler.UnitTests
{
    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void TestAttributeThresholdHandling()
        {
            var stylerOptions = new StylerOptions
            {
                AttributesTolerance = 0,
                MaxAttributeCharatersPerLine = 80,
                MaxAttributesPerLine = 3,
                PutEndingBracketOnNewLine = true
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeToleranceHandling()
        {
            var stylerOptions = new StylerOptions
            {
                AttributesTolerance = 3,
                RootElementLineBreakRule = LineBreakRule.Always,
            };

            DoTest(stylerOptions);
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

            DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestDefaultHandling()
        {
            DoTest();
        }

        [Test]
        public void TestAttributeSortingOptionHandling()
        {
            var stylerOptions = new StylerOptions
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

            DoTest(stylerOptions);
        }

        [Test]
        public void TestxBindSplitting()
        {
            var stylerOptions = new StylerOptions
            {
                NoNewLineMarkupExtensions = "x:Bind"
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestBindingSplitting()
        {
            var stylerOptions = new StylerOptions
            {
                NoNewLineMarkupExtensions = "x:Bind, Binding"
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestMarkupExtensionHandling()
        {
            var stylerOptions = new StylerOptions
            {
                FormatMarkupExtension = true
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestMarkupWithAttributeNotOnFirstLine()
        {
            var stylerOptions = new StylerOptions
            {
                KeepFirstAttributeOnSameLine = false,
                AttributesTolerance = 1
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestNoContentElementHandling()
        {
            DoTest();
        }

        [Test]
        public void TestTextOnlyContentElementHandling()
        {
            DoTest();
        }


        [Test]
        public void TestGridChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestNestedGridChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestCanvasChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestNestedCanvasChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestNestedPropertiesAndChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestAttributeOrderRuleGroupsOnSeparateLinesHandling()
        {
            var stylerOptions = new StylerOptions
            {
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                MaxAttributesPerLine = 3,
            };

            DoTest(stylerOptions);
        }

        [TestCase(ReorderSettersBy.Property)]
        [TestCase(ReorderSettersBy.TargetName)]
        [TestCase(ReorderSettersBy.TargetNameThenProperty)]
        public void TestReorderSetterHandling(ReorderSettersBy reorderSettersBy)
        {
            var stylerOptions = new StylerOptions
            {
                ReorderSetters = reorderSettersBy,
            };

            DoTestCase(stylerOptions, reorderSettersBy);
        }

        [TestCase(1, true)]
        [TestCase(2, false)]
        public void TestClosingElementHandling(int testNumber, bool spaceBeforeClosingSlash)
        {
            var stylerOptions = new StylerOptions
            {
                SpaceBeforeClosingSlash = spaceBeforeClosingSlash
            };

            DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestCDATAHandling()
        {
            DoTest();
        }

        [Test]
        public void TestXmlSpaceHandling()
        {
            DoTest();
        }

        [TestCase(1, LineBreakRule.Default)]
        [TestCase(2, LineBreakRule.Always)]
        [TestCase(3, LineBreakRule.Never)]
        public void TestRootHandling(int testNumber, LineBreakRule lineBreakRule)
        {
            var stylerOptions = new StylerOptions
            {
                AttributesTolerance = 3,
                MaxAttributesPerLine = 4,
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                RootElementLineBreakRule = lineBreakRule,
            };

            DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestRunHandling()
        {
            DoTest();
        }

        private void DoTest([System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            DoTest(new StylerOptions(), callerMemberName);
        }

        private void DoTest(StylerOptions stylerOptions, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), null);
        }

        //private void DoTest(StylerOptions stylerOptions, int testNumber, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        //{
        //    // ReSharper disable once ExplicitCallerInfoArgument
        //    DoTest(StylerService.CreateInstance(stylerOptions), testNumber, callerMemberName);
        //}

        /// <summary>
        /// Parse input document and verify output against 
        /// </summary>
        /// <param name="styler"></param>
        /// <param name="testIdentifier">Optional test identifier</param>
        /// <param name="callerMemberName"></param>
        private void DoTestCase<T>(StylerOptions stylerOptions, T testIdentifier, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), testIdentifier.ToString());
        }

        /// <summary>
        /// Style input document and verify output against expected  
        /// </summary>
        /// <param name="stylerOptions"></param>
        /// <param name="testFileBaseName"></param>
        /// <param name="suffix"></param>
        private static void DoTest(StylerOptions stylerOptions, string testFileBaseName, string suffix)
        {
            var stylerService = StylerService.CreateInstance(stylerOptions);
            
            var testFileResultBaseName = suffix != null ? testFileBaseName + "_" + suffix : testFileBaseName;

            // Excercise stylerService using supplied test xaml data
            string actualOutput = stylerService.ManipulateTreeAndFormatInput(File.ReadAllText(testFileBaseName + ".testxaml"));

            // Write output to ".actual" file for further investigation
            File.WriteAllText(testFileResultBaseName + ".actual", actualOutput, Encoding.UTF8);

            // Check result
            Assert.That(actualOutput, Is.EqualTo(File.ReadAllText(testFileResultBaseName + ".expected")));
        }
    }
}
