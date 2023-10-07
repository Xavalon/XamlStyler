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
                System.Console.Error.WriteLine(writer.ToString());
                Environment.Exit(1);
            })
            .WithParsed(options =>
            {
                ProcessType processType;
                if (!CheckOptions(options, out processType))
                {
                    Environment.Exit(1);
                }

                var xamlStylerConsole = new XamlStylerConsole(options);
                xamlStylerConsole.Process(processType);
            });

            return 0;
        }

        private static bool CheckOptions(CommandLineOptions options, out ProcessType processType)
        {
            if (options.LogLevel >= LogLevel.Debug)
            {
                System.Console.WriteLine($"File Parameter: '{options.File}'");
                System.Console.WriteLine($"File Count: {options.File?.Count ?? -1}");
                System.Console.WriteLine($"File Directory: '{options.Directory}'");
            }

            bool result = true;

            int numFilesSpecified = options.File?.Count ?? 0;
            bool isFileOptionSpecified = numFilesSpecified != 0;
            bool isDirectoryOptionSpecified = !String.IsNullOrEmpty(options.Directory);
            if (isFileOptionSpecified && isDirectoryOptionSpecified)
            {
                System.Console.Error.WriteLine($"\nError: Cannot specify both file(s) and directory\n");
                result = false;
            }
            else if (!isFileOptionSpecified && !isDirectoryOptionSpecified)
            {
                System.Console.Error.WriteLine($"\nError: Must specify file(s) or directory\n");
                result = false;
            }

            if (options.WriteToStdout && (isDirectoryOptionSpecified || numFilesSpecified != 1))
            {
                System.Console.Error.WriteLine($"\nError: When using --write-to-stdout you must specify exactly one file\n");
                result = false;
            }

            if (options.WriteToStdout && options.IsPassive)
            {
                System.Console.Error.WriteLine($"\nError: Cannot specify both --passive and --write-to-stdout\n");
                result = false;
            }

            processType = isFileOptionSpecified ? ProcessType.File : ProcessType.Directory;
            return result;
        }
    }
}
