using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Xmagic
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

            ApplyOptionOverrides(options, stylerOptions);

            this.stylerService = new StylerService(stylerOptions);
        }

        private void ApplyOptionOverrides(Options options, IStylerOptions stylerOptions)
        {
            if (options.IndentSize != null)
                stylerOptions.IndentSize = options.IndentSize.Value;

            if (options.IndentWithTabs != null)
                stylerOptions.IndentWithTabs = options.IndentWithTabs.Value;

            if (options.AttributesTolerance != null)
                stylerOptions.AttributesTolerance = options.AttributesTolerance.Value;

            if (options.KeepFirstAttributeOnSameLine != null)
                stylerOptions.KeepFirstAttributeOnSameLine = options.KeepFirstAttributeOnSameLine.Value;

            if (options.MaxAttributeCharactersPerLine != null)
                stylerOptions.MaxAttributeCharactersPerLine = options.MaxAttributeCharactersPerLine.Value;

            if (options.MaxAttributesPerLine != null)
                stylerOptions.MaxAttributesPerLine = options.MaxAttributesPerLine.Value;

            if (options.NoNewLineElements != null)
                stylerOptions.NoNewLineElements = options.NoNewLineElements;

            if (options.PutAttributeOrderRuleGroupsOnSeparateLines != null)
                stylerOptions.PutAttributeOrderRuleGroupsOnSeparateLines = options.PutAttributeOrderRuleGroupsOnSeparateLines.Value;

            if (options.AttributeIndentation != null)
                stylerOptions.AttributeIndentation = options.AttributeIndentation.Value;

            if (options.AttributeIndentationStyle != null)
                stylerOptions.AttributeIndentationStyle = options.AttributeIndentationStyle.Value;

            if (options.RemoveDesignTimeReferences != null)
                stylerOptions.RemoveDesignTimeReferences = options.RemoveDesignTimeReferences.Value;

            if (options.EnableAttributeReordering != null)
                stylerOptions.EnableAttributeReordering = options.EnableAttributeReordering.Value;

            if (options.FirstLineAttributes != null)
                stylerOptions.FirstLineAttributes = options.FirstLineAttributes;

            if (options.OrderAttributesByName != null)
                stylerOptions.OrderAttributesByName = options.OrderAttributesByName.Value;

            if (options.PutEndingBracketOnNewLine != null)
                stylerOptions.PutEndingBracketOnNewLine = options.PutEndingBracketOnNewLine.Value;

            if (options.RemoveEndingTagOfEmptyElement != null)
                stylerOptions.RemoveEndingTagOfEmptyElement = options.RemoveEndingTagOfEmptyElement.Value;

            if (options.RootElementLineBreakRule != null)
                stylerOptions.RootElementLineBreakRule = options.RootElementLineBreakRule.Value;

            if (options.ReorderVSM != null)
                stylerOptions.ReorderVSM = options.ReorderVSM.Value;

            if (options.ReorderGridChildren != null)
                stylerOptions.ReorderGridChildren = options.ReorderGridChildren.Value;

            if (options.ReorderCanvasChildren != null)
                stylerOptions.ReorderCanvasChildren = options.ReorderCanvasChildren.Value;

            if (options.ReorderSetters != null)
                stylerOptions.ReorderSetters = options.ReorderSetters.Value;

            if (options.FormatMarkupExtension != null)
                stylerOptions.FormatMarkupExtension = options.FormatMarkupExtension.Value;

            if (options.NoNewLineMarkupExtensions != null)
                stylerOptions.NoNewLineMarkupExtensions = options.NoNewLineMarkupExtensions;

            if (options.ThicknessStyle != null)
                stylerOptions.ThicknessStyle = options.ThicknessStyle.Value;

            if (options.ThicknessAttributes != null)
                stylerOptions.ThicknessAttributes = options.ThicknessAttributes;

            if (options.CommentSpaces != null)
                stylerOptions.CommentSpaces = options.CommentSpaces.Value;
        }

        public void Process(ProcessType processType)
        {
            int successCount = 0;

            IList<string> files;

            switch (processType)
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
}