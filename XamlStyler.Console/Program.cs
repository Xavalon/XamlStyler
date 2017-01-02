// © Xavalon. All rights reserved.

using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Xmagic
{
    public sealed class Program
    {
        public sealed class XamlStylerConsole
        {
            private readonly Options options;
            private readonly StylerService stylerService;

            public XamlStylerConsole(Options options)
            {
                this.options = options;

                IStylerOptions stylerOptions = new StylerOptions();

                if (this.options.Configuration != null)
                {
                    stylerOptions = this.LoadConfiguration(this.options.Configuration);
                }

                this.stylerService = new StylerService(stylerOptions);
            }

            public void Process(ProcessType processType)
            {
                int successCount = 0;

                IList<string> files;

                switch(processType)
                {
                    case ProcessType.File:
                        files = this.options.File;
                        break;
                    case ProcessType.Directory:
                        var searchOption = this.options.IsRecursive
                            ? SearchOption.AllDirectories
                            : SearchOption.TopDirectoryOnly;
                        files = (File.GetAttributes(this.options.Directory).HasFlag(FileAttributes.Directory))
                            ? Directory.GetFiles(this.options.Directory, "*.xaml", searchOption).ToList()
                            : new List<string>();
                        break;
                    default:
                        throw new ArgumentException("Invalid ProcessType");
                }

                foreach (string file in files)
                {
                    if (this.TryProcessFile(file))
                    {
                        successCount++;
                    }
                }

                this.Log($"Processed {successCount} of {files.Count} files.", LogLevel.Minimal);
            }

            private bool TryProcessFile(string file)
            {
                this.Log($"Processing: {file}");

                if (!options.Ignore)
                {
                    var extension = Path.GetExtension(file);
                    this.Log($"Extension: {extension}", LogLevel.Debug);

                    if (!extension.Equals(".xaml", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Log("Skipping... Can only process XAML files. Use the --ignore parameter to override.");
                        return false;
                    }
                }

                var path = Path.GetFullPath(file);
                this.Log($"Full Path: {file}", LogLevel.Debug);

                // If the options already has a configuration file set, we don't need to go hunting for one
                string configurationPath = string.IsNullOrEmpty(this.options.Configuration) ? this.GetConfigurationFromPath(path) : null;

                string originalContent = null;
                Encoding encoding = Encoding.UTF8; // Visual Studio by default uses UTF8
                using (var reader = new StreamReader(path))
                {
                    originalContent = reader.ReadToEnd();
                    encoding = reader.CurrentEncoding;
                    this.Log($"\nOriginal Content:\n\n{originalContent}\n", LogLevel.Insanity);
                }

                var formattedOutput = String.IsNullOrWhiteSpace(configurationPath)
                    ? stylerService.StyleDocument(originalContent)
                    : new StylerService(this.LoadConfiguration(configurationPath)).StyleDocument(originalContent);

                this.Log($"\nFormatted Output:\n\n{formattedOutput}\n", LogLevel.Insanity);

                using (var writer = new StreamWriter(path, false, encoding))
                {
                    try
                    {
                        writer.Write(formattedOutput);
                        this.Log($"Finished Processing: {file}", LogLevel.Verbose);
                    }
                    catch (Exception e)
                    {
                        this.Log("Skipping... Error formatting XAML. Increase log level for more details.");
                        this.Log($"Exception: {e.Message}", LogLevel.Verbose);
                        this.Log($"StackTrace: {e.StackTrace}", LogLevel.Debug);
                    }
                }

                return true;
            }

            private IStylerOptions LoadConfiguration(string path)
            {
                StylerOptions stylerOptions = new StylerOptions(path);
                this.Log(JsonConvert.SerializeObject(stylerOptions), LogLevel.Insanity);
                this.Log(JsonConvert.SerializeObject(stylerOptions.AttributeOrderingRuleGroups), LogLevel.Debug);
                return stylerOptions;
            }

            private string GetConfigurationFromPath(string path)
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(path))
                    {
                        return null;
                    }

                    bool isSolutionRoot = false;

                    while (!isSolutionRoot && ((path = Path.GetDirectoryName(path)) != null))
                    {
                        isSolutionRoot = Directory.Exists(Path.Combine(path, ".vs"));
                        this.Log($"In solution root: {isSolutionRoot}", LogLevel.Debug);
                        var configFile = Path.Combine(path, "Settings.XamlStyler");
                        this.Log($"Looking in: {path}", LogLevel.Debug);

                        if (File.Exists(configFile))
                        {
                            this.Log($"Configuration Found: {configFile}", LogLevel.Verbose);
                            return configFile;
                        }
                    }
                }
                catch
                {
                }

                return null;
            }

            private void Log(string value, LogLevel logLevel = LogLevel.Default)
            {
                if (logLevel <= this.options.LogLevel)
                {
                    Console.WriteLine(value);
                }
            }
        }

        public static void Main(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArgumentsStrict(args, options);

            if (options.LogLevel >= LogLevel.Debug)
            {
                Console.WriteLine($"File Parameter: '{options.File}'");
                Console.WriteLine($"File Count: {options.File?.Count ?? -1}");
                Console.WriteLine($"File Directory: '{options.Directory}'");
            }

            bool isFileOptionSpecified = ((options.File?.Count ?? 0) != 0);
            bool isDirectoryOptionSpecified = !String.IsNullOrEmpty(options.Directory);

            if (isFileOptionSpecified ^ isDirectoryOptionSpecified)
            {
                var xamlStylerConsole = new XamlStylerConsole(options);
                xamlStylerConsole.Process(isFileOptionSpecified ? ProcessType.File : ProcessType.Directory);
            }
            else
            {
                var errorString = (isFileOptionSpecified && isDirectoryOptionSpecified)
                    ? "Cannot specify both file(s) and directory"
                    : "Must specify file(s) or directory";

                Console.WriteLine($"\nError: {errorString}\n");
                Console.WriteLine(options.GetUsage());
            }
        }

        public sealed class Options
        {
            [OptionList('f', "file", Separator = ',', HelpText = "XAML file to process (supports comma-separated list).")]
            public IList<string> File { get; set; }

            [Option('d', "directory", HelpText = "Directory to process XAML files in.")]
            public string Directory { get; set; }

            [Option('c', "config", HelpText = "JSON file containing XAML Styler settings configuration.")]
            public string Configuration { get; set; }

            [Option('i', "ignore", DefaultValue = false, HelpText = "Ignore XAML file type check and process all files.")]
            public bool Ignore { get; set; }

            [Option('r', "recursive", DefaultValue = false, HelpText = "Recursively process specified directory.")]
            public bool IsRecursive { get; set; }

            [Option('l', "loglevel", DefaultValue = LogLevel.Default, HelpText = "Levels in order of increasing detail: None, Minimal, Default, Verbose, Debug")]
            public LogLevel LogLevel { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this);
            }
        }

        public enum LogLevel
        {
            None = 0,
            Minimal = 1,
            Default = 2,
            Verbose = 3,
            Debug = 4,
            Insanity = 5,
        }

        public enum ProcessType
        {
            File,
            Directory,
        }
    }
}