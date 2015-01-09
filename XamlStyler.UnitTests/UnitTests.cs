using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using XamlStyler.Core;
using XamlStyler.Core.Options;

namespace XamlStyler.UnitTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestAttributeThresholdHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            var stylerOptions = new StylerOptions
                                    {
                                        AttributesTolerance = 0,
                                        MaxAttributeCharatersPerLine = 80,
                                        MaxAttributesPerLine = 3,
                                        PutEndingBracketOnNewLine = true
                                    };

            var styler = StylerService.CreateInstance(stylerOptions);

            DoTest(testInput, styler);
        }

        [TestMethod]
        public void TestCommentHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }

        [TestMethod]
        public void TestDefaultHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }

        [TestMethod]
        public void TestAttributeSortingOptionHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

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
            var styler = StylerService.CreateInstance(stylerOptions);

            DoTest(testInput, styler);
        }

        [TestMethod]
        public void TestMarkupExtensionHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            var stylerOptions = new StylerOptions
                                    {
                                        FormatMarkupExtension = true
                                    };
            var styler = StylerService.CreateInstance(stylerOptions);

            DoTest(testInput, styler);
        }

        [TestMethod]
        public void TestNoContentElementHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }

        [TestMethod]
        public void TestTextOnlyContentElementHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }


        [TestMethod]
        public void TestGridChildrenHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }



        [TestMethod]
        public void TestNestedGridChildrenHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }



        [TestMethod]
        public void TestCanvasChildrenHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }
 

        [TestMethod]
        public void TestNestedCanvasChildrenHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }



        private void DoTest(string testInput)
        {
            DoTest(testInput, StylerService.CreateInstance(new StylerOptions()));
        }

        private void DoTest(string testInput, StylerService styler)
        {
            string actualOutputFile = testInput.Replace(".xaml", "_output.xaml");
            string expectedOutputFile = testInput.Replace(".xaml", "_output_expected.xaml");

            string output = styler.ManipulateTreeAndFormatInput(File.ReadAllText(testInput));

            File.WriteAllText(actualOutputFile, output);

            Assert.IsTrue(FileCompare(actualOutputFile, expectedOutputFile));
        }

        /// <summary>
        /// This method accepts two strings the represent two files to
        /// compare. A return value of 0 indicates that the contents of the files
        /// are the same. A return value of any other value indicates that the
        /// files are not the same.
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        private bool FileCompare(string file1, string file2)
        {
            int file1Byte;
            int file2Byte;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            var fs1 = new FileStream(file1, FileMode.Open);
            var fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1Byte = fs1.ReadByte();
                file2Byte = fs2.ReadByte();
            }
            while ((file1Byte == file2Byte) && (file1Byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is
            // equal to "file2byte" at this point only if the files are
            // the same.
            return ((file1Byte - file2Byte) == 0);
        }
    }
}