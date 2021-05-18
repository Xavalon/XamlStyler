// © Xavalon. All rights reserved.

using System.Collections.Generic;
using CommandLine;
using Xavalon.XamlStyler.DocumentManipulation;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Console
{
    // CLI-intrinsic options
    public sealed partial class CommandLineOptions
    {
        [Option('f', "file", Separator = ',', HelpText = "XAML file to process (supports comma-separated list).")]
        public IList<string> File { get; private set; }

        [Option('d', "directory", HelpText = "Directory to process XAML files in.")]
        public string Directory { get; set; }

        [Option('c', "config", HelpText = "JSON file containing XAML Styler settings configuration.")]
        public string Configuration { get; set; }

        [Option('i', "ignore", Default = false, HelpText = "Ignore XAML file type check and process all files.")]
        public bool Ignore { get; set; }

        [Option('r', "recursive", Default = false, HelpText = "Recursively process specified directory.")]
        public bool IsRecursive { get; set; }

        [Option('p', "passive", Default = false, HelpText = "Check files follow proper formatting without making any modifications.")]
        public bool IsPassive { get; set; }

        [Option('l', "loglevel", Default = LogLevel.Default,
            HelpText = "Levels in order of increasing detail: None, Minimal, Default, Verbose, Debug")]
        public LogLevel LogLevel { get; set; }
    }

    // Styler overrides
    public sealed partial class CommandLineOptions
    {
        [Option("indent-size", HelpText = "Override: indent size.")]
        public int? IndentSize { get; set; }

        [Option("indent-tabs", HelpText = "Override: indent with tabs.")]
        public bool? IndentWithTabs { get; set; }

        [Option("attributes-tolerance", HelpText = "Override: attributes tolerance.")]
        public int? AttributesTolerance { get; set; }

        [Option("attributes-same-line", HelpText = "Override: keep first attribute on the same line.")]
        public bool? KeepFirstAttributeOnSameLine { get; set; }

        [Option("attributes-max-chars", HelpText = "Override: max attribute characters per line.")]
        public int? MaxAttributeCharactersPerLine { get; set; }

        [Option("attributes-max", HelpText = "Override: max attributes per line.")]
        public int? MaxAttributesPerLine { get; set; }

        [Option("no-newline-elements", HelpText = "Override: no newline elements.")]
        public string NoNewLineElements { get; set; }

        [Option("attributes-order-groups-newline", HelpText = "Override: put attribute order rule groups on separate lines.")]
        public bool? PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        [Option("attributes-indentation", HelpText = "Override: attribute indentation.")]
        public int? AttributeIndentation { get; set; }

        [Option("attributes-indentation-style", HelpText = "Override: attribute indentation style.")]
        public AttributeIndentationStyle? AttributeIndentationStyle { get; set; }

        [Option("remove-design-references", HelpText = "Override: remove design time references.")]
        public bool? RemoveDesignTimeReferences { get; set; }

        [Option("attributes-reorder", HelpText = "Override: enable attribute reordering.")]
        public bool? EnableAttributeReordering { get; set; }

        [Option("attributes-first-line", HelpText = "Override: first line attributes.")]
        public string FirstLineAttributes { get; set; }

        [Option("attributes-order-name", HelpText = "Override: order attributes by name.")]
        public bool? OrderAttributesByName { get; set; }

        [Option("ending-bracket-newline", HelpText = "Override: put ending bracket on new line.")]
        public bool? PutEndingBracketOnNewLine { get; set; }

        [Option("remove-empty-ending-tag", HelpText = "Override: remove ending tag of empty element.")]
        public bool? RemoveEndingTagOfEmptyElement { get; set; }

        [Option("space-before-closing-slash", HelpText = "Override: space before closing slash.")]
        public bool? SpaceBeforeClosingSlash { get; set; }

        [Option("root-line-break", HelpText = "Override: root element line break rule.")]
        public LineBreakRule? RootElementLineBreakRule { get; set; }

        [Option("reorder-vsm", HelpText = "Override: reorder visual state manager rule.")]
        public VisualStateManagerRule? ReorderVSM { get; set; }

        [Option("reorder-grid-children", HelpText = "Override: reorder grid children.")]
        public bool? ReorderGridChildren { get; set; }

        [Option("reorder-canvas-children", HelpText = "Override: reorder canvas children.")]
        public bool? ReorderCanvasChildren { get; set; }

        [Option("reorder-setters", HelpText = "Override: reorder setters.")]
        public ReorderSettersBy? ReorderSetters { get; set; }

        [Option("format-markup-extension", HelpText = "Override: format markup extension.")]
        public bool? FormatMarkupExtension { get; set; }

        [Option("no-newline-markup-extensions", HelpText = "Override: no newline markup extensions.")]
        public string NoNewLineMarkupExtensions { get; set; }

        [Option("thickness-style", HelpText = "Override: thickness style.")]
        public ThicknessStyle? ThicknessStyle { get; set; }

        [Option("thickness-attributes", HelpText = "Override: thickness attributes.")]
        public string ThicknessAttributes { get; set; }

        [Option("comment-spaces", HelpText = "Override: comment spaces.")]
        public int? CommentSpaces { get; set; }

        [Option("end-of-line", HelpText = "Override: end-of-line.")]
        public string EndOfLine { get; set; }
    }
}