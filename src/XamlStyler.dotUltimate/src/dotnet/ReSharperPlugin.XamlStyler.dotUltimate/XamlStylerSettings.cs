using JetBrains.Application.Settings;
using JetBrains.ReSharper.Resources.Settings;
using JetBrains.Util;
using Xavalon.XamlStyler.Core.DocumentManipulation;
using Xavalon.XamlStyler.Core.Options;

namespace ReSharperPlugin.XamlStyler.dotUltimate
{
    [SettingsKey(typeof(CodeStyleSettings), "Settings for XAML Styler")]
    public class XamlStylerSettings
    {
        #region Indentation

        [SettingsEntry(DefaultValue: 4, Description: "Indent size")]
        public int IndentSize { get; set; }

        [SettingsEntry(DefaultValue: true, Description: "Use IDE value")]
        public bool UseIdeIndentSize { get; }

        [SettingsEntry(DefaultValue: false, Description: "Indent with tabs")]
        public bool IndentWithTabs { get; set; }

        [SettingsEntry(DefaultValue: true, Description: "Use IDE value")]
        public bool UseIdeIndentWithTabs { get; }

        #endregion Indentation

        #region Attribute formatting
        
        [SettingsEntry(DefaultValue: 2, Description: "Defines the maximum number of attributes allowed on a single line. If the number of attributes exceeds this value, XAML Styler will break the attributes up across multiple lines. A value of less than 1 means always break up the attributes.\nDefault Value: 2")]
        public int AttributesTolerance { get; set; }

        [SettingsEntry(DefaultValue: false, Description: "Defines whether the first line of attribute(s) should appear on the same line as the element's start tag.\nDefault Value: false")]
        public bool KeepFirstAttributeOnSameLine { get; set; }

        [SettingsEntry(DefaultValue: 0, Description: "Defines the maximum character length of attributes an element can have on each line after the start tag (not including indentation characters). A value of less than 1 means no limit. Note: This setting only takes effect if Max Attributes Per Line is greater than 1 and Attribute Tolerance is greater than 2.\nDefault Value: 0")]
        public int MaxAttributeCharactersPerLine { get; set; }

        [SettingsEntry(DefaultValue: 1, Description: "Defines the maximum number of attributes an element can have on each line after the start tag if the number of attributes exceeds the attribute tolerance. A value of less than 1 means no limit.\nDefault Value: 1")]
        public int MaxAttributesPerLine { get; set; }

        [SettingsEntry(
            DefaultValue: "RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransform, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter",
            Description: "Defines a list of elements whose attributes should not be broken across lines.\nDefault Value: RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransform, SkewTransform, RotateTransform, TranslateTransform, Trigger, Setter")]
        public string NoNewLineElements { get; set; }

        [SettingsEntry(DefaultValue: false, Description: "Defines whether attributes belonging to different rule groups should be put on separate lines, while, if possible, keeping attributes in the same group on the same line.\nDefault Value: false")]
        public bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        [SettingsEntry(DefaultValue: 0, Description: "Defines the number of spaces that attributes are indented on elements with more than one line of attributes. A value of 0 will align indentation with the first attribute.\nDefault Value: 0")]
        public int AttributeIndentation { get; set; }

        [SettingsEntry(DefaultValue: AttributeIndentationStyle.Spaces, Description: "Defines how attributes are indented.\nDefault Value: Spaces")]
        public AttributeIndentationStyle AttributeIndentationStyle { get; set; }

        [SettingsEntry(DefaultValue: false, Description: "Defines whether design-time references automatically added to new pages and controls should be removed.\nDefault Value: false")]
        public bool RemoveDesignTimeReferences { get; set; }

        #endregion Attribute formatting

        #region Attribute Reordering

        [SettingsEntry(DefaultValue: true, Description: "Defines whether attributes should be reordered. If false, attributes will not be reordered in any way.\nDefault Value: true")]
        public bool EnableAttributeReordering { get; set; }

        [SettingsEntry(DefaultValue: "", Description: "Defines attribute ordering rule groups. Each string element is one group. Use ',' as a delimiter between attributes. 'DOS' wildcards are allowed. XAML Styler will order attributes in groups from top to bottom, and within groups left to right.")]
        public string AttributeOrderingRuleGroups { get; set; }

        [SettingsEntry(DefaultValue: "", Description: "Defines a list of attributes which should always appear on the same line as the element's start tag. Attribute reordering must be enabled for this setting to take effect.\nDefault Value: None")]
        public string FirstLineAttributes { get; set; }

        [SettingsEntry(DefaultValue: true, Description: "Defines whether attributes should be ordered by name if not determined by a rule.\nDefault Value: true")]
        public bool OrderAttributesByName { get; set; }

        #endregion Attribute Reordering

        #region Element formatting
        
        [SettingsEntry(DefaultValue: false, Description: "Defines whether to put ending brackets on a new line.\nDefault Value: false")]
        public bool PutEndingBracketOnNewLine { get; set; }

        [SettingsEntry(DefaultValue: true, Description: "Defines whether to remove the end tag of an empty element.\nDefault Value: true")]
        public bool RemoveEndingTagOfEmptyElement { get; set; }

        [SettingsEntry(DefaultValue: true, Description: "Defines whether there should be a space before the slash in ending brackets for self-closing elements.\nDefault Value: true")]
        public bool SpaceBeforeClosingSlash { get; set; }

        [SettingsEntry(DefaultValue: LineBreakRule.Default, Description: "Defines whether attributes of the document root element are broken into multiple lines.\nDefault Value: Default (use same rules as other elements)")]
        public LineBreakRule RootElementLineBreakRule { get; set; }

        #endregion Element formatting

        #region Element reordering

        [SettingsEntry(DefaultValue: VisualStateManagerRule.Last, Description: "Defines whether to reorder the visual state manager. When set to first or last, the visual state manager will be moved to the first or last child element in its parent, respectively, otherwise it will not be moved.\nDefault Value: Last")]
        public VisualStateManagerRule ReorderVSM { get; set; }

        [SettingsEntry(DefaultValue: false, Description: "Defines whether to reorder the children of a Grid by row/column. When true, children will be reordered in an ascending fashion by looking first at Grid.Row, then by Grid.Column.\nDefault Value: false")]
        public bool ReorderGridChildren { get; set; }

        [SettingsEntry(DefaultValue: false, Description: "Defines whether to reorder the children of a Canvas by left/top/right/bottom. When true, children will be reordered in an ascending fashion by first at Canvas.Left, then by Canvas.Top, Canvas.Right, and finally, Canvas.Bottom.\nDefault Value: false")]
        public bool ReorderCanvasChildren { get; set; }

        [SettingsEntry(DefaultValue: ReorderSettersBy.None, Description: "Defines whether to reorder 'Setter' elements in style/trigger elements. When this is set, children will be reordered in an ascending fashion by looking at their Property and/or TargetName properties.\nDefault Value: None")]
        public ReorderSettersBy ReorderSetters { get; set; }

        #endregion Element reordering

        #region Markup Extension

        [SettingsEntry(DefaultValue: true, Description: "Defines whether to format Markup Extensions (attributes containing '{}'). When true, attributes with markup extensions will always be put on a new line, unless the element is under the attribute tolerance or one of the specified elements is in the list of elements with no line breaks between attributes.\nDefault Value: true")]
        public bool FormatMarkupExtension { get; set; }

        [SettingsEntry(DefaultValue: "x:Bind, Binding", Description: "Defines a comma-separated list of Markup Extensions that are always kept on a single line\nDefault Value: x:Bind, Binding")]
        public string NoNewLineMarkupExtensions { get; set; }

        #endregion Markup Extension

        #region Thickness formatting

        [SettingsEntry(DefaultValue: ThicknessStyle.Comma, Description: "Defines how thickness attributes (i.e., margin, padding, etc.) should be formatted.\nDefault Value: Comma")]
        public ThicknessStyle ThicknessStyle { get; set; }

        [SettingsEntry(DefaultValue: "Margin, Padding, BorderThickness, ThumbnailClipMargin", Description: "Defines a list of attributes that get reformatted if content appears to be a thickness.\nDefault Value: Margin, Padding, BorderThickness, ThumbnailClipMargin")]
        public string ThicknessAttributes { get; set; }

        #endregion Thickness formatting

        #region Misc

        [SettingsEntry(DefaultValue: true, Description: "Defines whether to automatically format the active XAML document while saving.\nDefault Value: true")]
        public bool FormatOnSave { get; set; }

        [SettingsEntry(DefaultValue: 2, Description: "Determines the number of spaces a XAML comment should be padded with.\nDefault Value: 2")]
        public int CommentSpaces { get; set; }

        #endregion Misc

        #region Configuration

        [SettingsEntry(DefaultValue: false, Description: "Defines location of external XAML Styler configuration file. Specifying an external configuration file allows you to easily point multiple instances to a shared configuration. The configuration path can be local or network-based. Invalid configurations will be ignored.\nDefault Value: N/A")]
        public FileSystemPath ConfigPath { get; set; }

        [SettingsEntry(DefaultValue: false, Description: "When set to true, XAML Styler will look for an external XAML Styler configuration file not only up through your solution directory, but up through the drives root of the current solution so you can share one configuration file through multiple solutions.\nDefault Value: false")]
        public bool SearchToDriveRoot { get; set; }
        
        [SettingsEntry(DefaultValue: false, Description: "Suppress processing\nDefault Value: false")]
        public bool SuppressProcessing { get; set; }

        #endregion Configuration
    }
}