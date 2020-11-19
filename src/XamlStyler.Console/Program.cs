// © Xavalon. All rights reserved.

using CommandLine;
using System;
using System.IO;

namespace Xavalon.XamlStyler.Console
{
    public sealed partial class Program
    {
        public static void Main(string[] args)
        {
            args = new string[]
            {
                // Provide sample test file to "xaml-style".
                "-f", @"C:\Users\developer\Desktop\XamlStylerTests\SampleXaml.xaml",
                // Set Debug level logging.
                "-l", "Debug",
                // XAML styler settings file.
                "-c", @"C:\Users\developer\Desktop\XamlStylerTests\StylerSettings.json",
            };
            var writer = new StringWriter();
            var parser = new CommandLine.Parser(_ => _.HelpWriter = writer);
            ParserResult<CommandLineOptions> result = parser.ParseArguments<CommandLineOptions>(args);

            result.WithNotParsed(_ =>
            {
                System.Console.WriteLine(writer.ToString());
                Environment.Exit(1);
            })
            .WithParsed(options =>
            {
                if (options.LogLevel >= LogLevel.Debug)
                {
                    System.Console.WriteLine($"File Parameter: '{options.File}'");
                    System.Console.WriteLine($"File Count: {options.File?.Count ?? -1}");
                    System.Console.WriteLine($"File Directory: '{options.Directory}'");
                }

                bool isFileOptionSpecified = (options.File?.Count ?? 0) != 0;
                bool isDirectoryOptionSpecified = !String.IsNullOrEmpty(options.Directory);

                if (isFileOptionSpecified ^ isDirectoryOptionSpecified)
                {
                    var xamlStylerConsole = new XamlStylerConsole(options);
                    xamlStylerConsole.Process(isFileOptionSpecified ? ProcessType.File : ProcessType.Directory);
                }
                else
                {
                    string errorString = (isFileOptionSpecified && isDirectoryOptionSpecified)
                        ? "Cannot specify both file(s) and directory"
                        : "Must specify file(s) or directory";

                    System.Console.WriteLine($"\nError: {errorString}\n");
                }
            });
        }
    }
}