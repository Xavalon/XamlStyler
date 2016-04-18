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

        [Test]
        public void TestCommentHandling()
        {
            DoTest();
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
                AttributeOrderClass = "x:Class",
                AttributeOrderWpfNamespace = "xmlns, xmlns:x",
                AttributeOrderKey = "Key, x:Key, Uid, x:Uid",
                AttributeOrderName = "Name, x:Name, Title",
                AttributeOrderAttachedLayout =
                    "Grid.Column, Grid.ColumnSpan, Grid.Row, Grid.RowSpan, Canvas.Right, Canvas.Bottom, Canvas.Left, Canvas.Top",
                AttributeOrderCoreLayout =
                    "MinWidth, MinHeight, Width, Height, MaxWidth, MaxHeight, Margin",
                AttributeOrderAlignmentLayout =
                    "Panel.ZIndex, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment",
                AttributeOrderOthers =
                    "Offset, Color, TargetName, Property, Value, StartPoint, EndPoint, PageSource, PageIndex",
                AttributeOrderBlendRelated =
                    "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText"
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestxBindSplitting()
        {
            var stylerOptions = new StylerOptions
            {
                KeepxBindOnSameLine = true
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestBindingSplitting()
        {
            var stylerOptions = new StylerOptions
            {
                KeepBindingsOnSameLine = true
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

        [TestCase(1, ReorderSettersBy.Property)]
        [TestCase(2, ReorderSettersBy.TargetName)]
        [TestCase(3, ReorderSettersBy.TargetNameThenProperty)]
        public void TestReorderSetterHandling(int testNumber, ReorderSettersBy reorderSettersBy)
        {
            var stylerOptions = new StylerOptions
            {
                ReorderSetters = reorderSettersBy,
            };

            DoTest(stylerOptions, testNumber);
        }

        [TestCase(1, true)]
        [TestCase(2, false)]
        public void TestClosingElementHandling(int testNumber, bool spaceBeforeClosingSlash)
        {
            var stylerOptions = new StylerOptions
            {
                SpaceBeforeClosingSlash = spaceBeforeClosingSlash
            };

            DoTest(stylerOptions, testNumber);
        }

        [Test]
        public void TestCDATAHandling()
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
            // ReSharper disable once ExplicitCallerInfoArgument
            DoTest(StylerService.CreateInstance(stylerOptions), 0, callerMemberName);
        }

        private void DoTest(StylerOptions stylerOptions, int testNumber, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            DoTest(StylerService.CreateInstance(stylerOptions), testNumber, callerMemberName);
        }

        /// <summary>
        /// Parse input document and verify output against 
        /// </summary>
        /// <param name="styler"></param>
        /// <param name="testNumber"></param>
        /// <param name="callerMemberName"></param>
        private void DoTest(StylerService styler, int testNumber, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            var testFileBaseName = Path.Combine("TestFiles", callerMemberName);
            var testFileResultBaseName = testNumber == 0 ? testFileBaseName : testFileBaseName + "_" + testNumber;

            // Excercise stylerService using supplied test xaml data
            string actualOutput = styler.ManipulateTreeAndFormatInput(File.ReadAllText(testFileBaseName + ".testxaml"));

            // Write output to ".actual" file for further investigation
            File.WriteAllText(testFileResultBaseName + ".actual", actualOutput, Encoding.UTF8);

            // Check result
            Assert.That(actualOutput, Is.EqualTo(File.ReadAllText(testFileResultBaseName + ".expected")));
        }
    }
}
