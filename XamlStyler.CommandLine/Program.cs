using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            if (!File.Exists(executeOptions.XamlFile))
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
            string originalContent;
            Encoding encoding = Encoding.UTF8; // Visual Studio by default uses UTF8
            using (var reader = new StreamReader(executeOptions.XamlFile))
            {
                originalContent = reader.ReadToEnd();
                encoding = reader.CurrentEncoding;
            }
            var formattedOutput = service.StyleDocument(originalContent);
            using (var writer = new StreamWriter(executeOptions.OutputXamlFile, false, encoding))
                writer.Write(formattedOutput);

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