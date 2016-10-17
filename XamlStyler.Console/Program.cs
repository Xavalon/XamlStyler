// © Xavalon. All rights reserved.

using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

            public void Process()
            {
                int successCount = 0;

                foreach (string file in this.options.Files)
                {
                    this.Log($"Processing: {file}");

                    if (!options.Ignore)
                    {
                        var extension = Path.GetExtension(file);
                        this.Log($"Extension: {extension}", LogLevel.Debug);

                        if (!extension.Equals(".xaml", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Log("Skipping... Can only process XAML files. Use the --ignore parameter to override.");
                            continue;
                        }
                    }

                    var path = Path.GetFullPath(file);
                    this.Log($"Full Path: {file}", LogLevel.Debug);

                    string configurationPath = this.GetConfigurationFromPath(path);

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
                            successCount++;
                        }
                        catch (Exception e)
                        {
                            this.Log("Skipping... Error formatting XAML. Increase log level for more details.");
                            this.Log($"Exception: {e.Message}", LogLevel.Verbose);
                            this.Log($"StackTrace: {e.StackTrace}", LogLevel.Debug);
                        }
                    }
                }

                this.Log($"Processed {successCount} of {this.options.Files.Count} files.", LogLevel.Minimal);
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
                        this.Log($"In solution root: {isSolutionRoot}");
                        var configFile = Path.Combine(path, "Settings.XamlStyler");
                        this.Log($"Looking in: {path}");

                        if (File.Exists(configFile))
                        {
                            this.Log($"Configuration Found: {configFile}");
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
            var xamlStylerConsole = new XamlStylerConsole(options);
            xamlStylerConsole.Process();
        }

        public sealed class Options
        {
            [OptionList('f', "files", Separator = ',', Required = true, HelpText = "XAML files to process.")]
            public IList<string> Files { get; set; }

            [Option('c', "config", HelpText = "JSON file containing XAML Styler settings configuration.")]
            public string Configuration { get; set; }

            [Option('i', "ignore", DefaultValue = false, HelpText = "Ignore XAML file type check and process all files.")]
            public bool Ignore { get; set; }

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
    }
}