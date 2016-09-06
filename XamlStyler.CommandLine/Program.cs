using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;

namespace XamlStyler.CommandLine
{
    class Program
    {
        static int Main(string[] args)
        {
            PrintHeader();
            if (args.Length == 0)
            {
                PrintHelp();
                return 0;
            }
            var executeOptions = GetOptions(new Queue<string>(args));
            if(!File.Exists(executeOptions.XamlFile))
            {
                Console.WriteLine($"File not found \"{executeOptions.XamlFile}\"");
                return 1;
            }
            if (!string.IsNullOrWhiteSpace(executeOptions.SettingFile) && !File.Exists(executeOptions.SettingFile))
            {
                Console.WriteLine($"File settings not found \"{executeOptions.SettingFile}\"");
                return 1;
            }
            var xamlOptions = new StylerOptions(executeOptions.SettingFile);
            var service = new StylerService(xamlOptions);
            var result = service.StyleDocument(File.ReadAllText(executeOptions.XamlFile));
            File.WriteAllText(executeOptions.OutputXamlFile, result);
            return 0;
        }
        private static Options GetOptions(Queue<string> args)
        {
            var options = new Options
            {
                XamlFile = args.Dequeue()
            };
            while (args.Any())
            {
                var key = args.Dequeue();
                switch (key)
                {
                    case "-s":
                        options.SettingFile = args.Dequeue();
                    break;
                    case "-o":
                        options.OutputXamlFile = args.Dequeue();
                        break;
                    default:
                        Console.WriteLine($"Unknown key: \"{key}\"");
                    break;
                }
            }
            if (options.OutputXamlFile == null)
                options.OutputXamlFile = options.XamlFile;
            return options;
        }
        private static void PrintHelp()
        {
            Console.WriteLine("usage: XamlStyler <xaml-file> [-o <output-xaml-file>] [-s <setings-file>]");
        }
        private static void PrintHeader()
        {
            var version = typeof(Program).Assembly.GetName().Version;
            Console.WriteLine($"XamlStyler Version: {version}, Command line utility for styling xaml");
        }
    }
}
