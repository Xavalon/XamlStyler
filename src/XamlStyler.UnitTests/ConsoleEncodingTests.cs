// © Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Xavalon.XamlStyler.Console;
using Xavalon.XamlStyler.DocumentManipulation;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.UnitTests
{
    [TestFixture]
    [NonParallelizable]
    public class ConsoleEncodingTests
    {
        private const string UnformattedXaml = "<TextBlock Text=\"Gr\u00FC\u00DFe \u4E16\u754C \U0001F600\"/>";
        private string directory;
        private string path;

        private static IEnumerable<Encoding> Encodings
        {
            get
            {
                yield return new UTF8Encoding(false);
                yield return new UTF8Encoding(true);
                yield return new UnicodeEncoding(false, true);
                yield return new UnicodeEncoding(true, true);
                yield return new UTF32Encoding(false, true);
                yield return new UTF32Encoding(true, true);
            }
        }

        [SetUp]
        public void SetUp()
        {
            this.directory = Path.Combine(Path.GetTempPath(), "XamlStylerEncodingTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this.directory);
            this.path = Path.Combine(this.directory, "Test.xaml");
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(this.directory, true);
        }

        [TestCaseSource(nameof(Encodings))]
        public void FormattingPreservesEncodingAndPreamble(Encoding encoding)
        {
            File.WriteAllText(this.path, UnformattedXaml, encoding);
            byte[] original = File.ReadAllBytes(this.path);

            Assert.That(Program.Main(new[] { "--file", this.path, "--loglevel", "None" }), Is.Zero);

            byte[] formatted = File.ReadAllBytes(this.path);
            Assert.That(formatted, Is.Not.EqualTo(original), "The test must exercise a file rewrite.");
            Assert.That(formatted, Is.EqualTo(GetBytes(encoding, GetFormattedXaml())));

            Assert.That(Program.Main(new[] { "--file", this.path, "--loglevel", "None" }), Is.Zero);
            Assert.That(File.ReadAllBytes(this.path), Is.EqualTo(formatted), "A second pass must preserve every byte.");
        }

        [TestCaseSource(nameof(Encodings))]
        public void PassiveCheckDoesNotChangeEncodingOrContent(Encoding encoding)
        {
            File.WriteAllText(this.path, GetFormattedXaml(), encoding);
            byte[] original = File.ReadAllBytes(this.path);

            Assert.That(Program.Main(new[] { "--file", this.path, "--passive", "--loglevel", "None" }), Is.Zero);

            Assert.That(File.ReadAllBytes(this.path), Is.EqualTo(original));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void StandardOutputPreservesPreambleAndLeavesFileUnchanged(bool emitBom)
        {
            var encoding = new UTF8Encoding(emitBom);
            File.WriteAllText(this.path, UnformattedXaml, encoding);
            byte[] original = File.ReadAllBytes(this.path);
            TextWriter previousOutput = System.Console.Out;
            Encoding previousEncoding = System.Console.OutputEncoding;
            using var output = new StringWriter();

            try
            {
                System.Console.SetOut(output);

                Assert.That(Program.Main(new[] { "--file", this.path, "--write-to-stdout", "--loglevel", "None" }), Is.Zero);
                Assert.That(output.ToString(), Is.EqualTo(encoding.GetString(encoding.GetPreamble()) + GetFormattedXaml()));
                Assert.That(System.Console.OutputEncoding.CodePage, Is.EqualTo(previousEncoding.CodePage));
            }
            finally
            {
                System.Console.OutputEncoding = previousEncoding;
                System.Console.SetOut(previousOutput);
            }

            Assert.That(File.ReadAllBytes(this.path), Is.EqualTo(original));
        }

        private static string GetFormattedXaml()
        {
            return new StylerService(new StylerOptions(), new XamlLanguageOptions { IsFormatable = true })
                .StyleDocument(UnformattedXaml);
        }

        private static byte[] GetBytes(Encoding encoding, string text)
        {
            return encoding.GetPreamble().Concat(encoding.GetBytes(text)).ToArray();
        }
    }
}
