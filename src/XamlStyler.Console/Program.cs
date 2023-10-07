// © Xavalon. All rights reserved.

using CommandLine;
using System;
using System.IO;

namespace Xavalon.XamlStyler.Console
{
    public sealed partial class Program
    {
        public static int Main(string[] args)
        {
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

                int numFilesSpecified = options.File?.Count ?? 0;
                bool isFileOptionSpecified = numFilesSpecified != 0;
                bool isDirectoryOptionSpecified = !String.IsNullOrEmpty(options.Directory);

                if (options.WriteToStdout && (isDirectoryOptionSpecified || numFilesSpecified != 1))
                {
                    System.Console.WriteLine($"\nError: When using --write-to-stdout you must specify exactly one file\n");
                }
                else if (isFileOptionSpecified ^ isDirectoryOptionSpecified)
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

                    Environment.Exit(1);
                }
            });

            return 0;
        }
    }
}