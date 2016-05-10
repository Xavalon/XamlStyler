// © Xavalon. All rights reserved.

using System.ComponentModel;
using Xavalon.XamlStyler.Core.DocumentManipulation;

namespace Xavalon.XamlStyler.Core.Options
{
    public class StylerOptions : IStylerOptions
    {
        public StylerOptions()
        {
            // Initialize all properties with "DefaultValueAttrbute" to their default value.
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                // Set default value if DefaultValueAttribute is present.
                DefaultValueAttribute attribute =
                    property.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

                if (attribute != null)
                {
                    property.SetValue(this, attribute.Value);
                }
            }
        }

        // Indentation
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(2)]
        [Browsable(false)]
        public int IndentSize { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Browsable(false)]
        public bool IndentWithTabs { get; set; }

        // Attribute formatting
        [Category("Attribute Formatting")]
        [DisplayName("Attribute tolerance")]
        [Description(@"Defines the attribute number tolerance before XamlStyler starts to break attributes into new lines. 0 = no tolerance.
e.g., when this setting is 2

BEFORE BEAUTIFY:
<TextBlock x:Name=""m_sample""
    Text=""asdf"" />

AFTER BEAUTIFY:
<TextBlock x:Name=""m_sample"" Text=""asdf"" />
Default Value: 2")]
        [DefaultValue((byte)2)]
        public byte AttributesTolerance { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Position first attribute on same line as start tag")]
        [Description(@"Defines whether the first line of attribute(s) shall appear on the same line as the element's start tag.
e.g., when this setting is true

BEFORE BEAUTIFY:
""<element a='xyz' b='xyz'>  </element>""

AFTER BEAUTIFY:
""<element a='xyz'
        b='xyz'>
</element>""

Default Value: true")]
        [DefaultValue(true)]
        public bool KeepFirstAttributeOnSameLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Max attribute characters per line")]
        [Description("Defines the maximum character length (not including indentation characters) of attributes an element can have on each line after the start tag. A value less than 1 meaning no limit. \r\nDefault Value: 0")]
        [DefaultValue(0)]
        public int MaxAttributeCharatersPerLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Max attributes per line")]
        [Description(@"Defines the maximum number of attributes an element can have on each line after the start tag. 0 = no limit.
Default Value: 1")]
        [DefaultValue((byte)1)]
        public byte MaxAttributesPerLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Elements no line break between attributes")]
        [Description(@"Defines a list of elements whose attributes shall not be broken into lines.
Default Value: RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Setter")]
        [DefaultValue("RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter")]
        public string NoNewLineElements { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Put Rule Groups on separate Lines")]
        [Description("Put attributes belonging to different rule groups on different lines and keep identical groups on same line if possible")]
        [DefaultValue(false)]
        public bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Attribute indentation")]
        [Description(@"Defines the number of spaces that attributes are indented, on elements with more than one line of attributes.
0 = align with first attribute.")]
        [DefaultValue((byte)0)]
        public byte AttributeIndentation { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Attribute indentation style")]
        [Description(@"Defines how attributes are indented after Element indentation has been applied. 
Mixed = Tabs (if using tabs) then spaces.
Spaces = Always use spaces.")]
        [DefaultValue(AttributeIndentationStyle.Spaces)]
        public AttributeIndentationStyle AttributeIndentationStyle { get; set; }

        // Attribute Reordering
        [Category("Attribute Reordering")]
        [DisplayName("Enable Attribute Reordering")]
        [Description("If this is disabled, attributes will not be reordered in any way.")]
        [DefaultValue(true)]
        public bool EnableAttributeReordering { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("Attribute Ordering Rule Groups")]
        [Description(@"Defines attribute ordering rule groups. Each string element is one group.
Use ',' to separate more than one attribute.'DOS' wildcards are allowed.
Attributes listed in earlier groups takes precedence over later groups.
Attributes listed earlier in same group takes precedence over the ones listed later.")]
        [DefaultValue(new[]
        {
            // Class definition group
            "x:Class",
            // WPF Namespaces group
            "xmlns, xmlns:x",
            // Other namespace
            "xmlns:*",
            // Element key group
            "Key, x:Key, Uid, x:Uid",
            // Element name group
            "Name, x:Name, Title",
            // Attached layout group
            "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
            // Core layout group
            "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight, Margin",
            // Alignment layout group
            "HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex",
            // Unmatched
            "*:*, *",
            // Miscellaneous/Other attributes group
            "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint",
            // Blend related group
            "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText",
        })]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] AttributeOrderingRuleGroups { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("Order Attributes by name")]
        [Description("Order Attributes by name if order is not determined by Rules")]
        [DefaultValue(true)]
        public bool OrderAttributesByName { get; set; }

        // Element formatting
        [Category("Element Formatting")]
        [DisplayName("Put ending bracket on new line")]
        [Description(@"Defines whether to put "">"" or ""/>"" on a new line.
Default Value: false")]
        [DefaultValue(false)]
        public bool PutEndingBracketOnNewLine { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Remove ending tag of empty element")]
        [Description(@"Defines whether to remove the ending tag of an empty element.
e.g., when this setting is true

BEFORE BEAUTIFY:
""<element>  </element>""

AFTER BEAUTIFY:
""<element />""

Default Value: true")]
        [DefaultValue(true)]
        public bool RemoveEndingTagOfEmptyElement { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Space before closing slash in self closing element")]
        [Description(@"Defines whether to have a space before slash in self closing elements
e.g., when
true <br />
false <br/>

Default Value: true")]
        [DefaultValue(true)]
        public bool SpaceBeforeClosingSlash { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Root element line breaks between attributes")]
        [Description(@"Defines if attributes of the document root element are broken into separate lines or not.
Default = use same rules as other elements")]
        [DefaultValue(LineBreakRule.Default)]
        public LineBreakRule RootElementLineBreakRule { get; set; }

        // Element reordering
        [Category("Element Reordering")]
        [DisplayName("Reorder Grid panel children by row/column")]
        [Description("Defines whether to reorder the children of a Grid by row/column. When this is true, children will be reordered in an ascending fashion by looking at their attached Grid properties: first by Grid.Row, then by Grid.Column.")]
        [DefaultValue(true)]
        public bool ReorderGridChildren { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder Canvas panel children by left/top/right/bottom")]
        [Description("Defines whether to reorder the children of a Canvas by left/top/right/bottom.  When this is true, children will be reordered in an ascending fashion by looking at their attached Canvas properties: first by Canvas.Left, then by Canvas.Top, then by Canvas.Right, then by Canvas.Bottom.")]
        [DefaultValue(true)]
        public bool ReorderCanvasChildren { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder Setters by")]
        [Description("Defines whether to reorder 'Setter' elements in style/trigger elements. When this is set, children will be reordered in an ascending fashion by looking at their Property and/or TargetName properties")]
        [DefaultValue(ReorderSettersBy.None)]
        public ReorderSettersBy ReorderSetters { get; set; }

        // Markup Extension
        [Category("Markup Extension")]
        [DisplayName("Enable Markup Extension Formatting")]
        [Description(@"Defines whether to format markup extensions (attributes containing '{}').
Default Value: true
When this setting is true, attributes with markup extensions will always be put on a new line, UNLESS the element is under AttributesTolerance or one of the NoNewLineElements.")]
        [DefaultValue(true)]
        public bool FormatMarkupExtension { get; set; }

        [Category("Markup Extension")]
        [DisplayName("Keep Markup Extensions of these types on one line")]
        [Description("Defines a comma separated list of Markup Extensions that are always kept on one line")]
        [DefaultValue("x:Bind")]
        public string NoNewLineMarkupExtensions { get; set; }

        // Thickness formatting
        [Category("Thickness formatting")]
        [DisplayName("Thickness style")]
        [Description("Defines how Thickness properties like Margin, Padding etc. should be formatted")]
        [DefaultValue(ThicknessStyle.None)]
        public ThicknessStyle ThicknessStyle { get; set; }

        [Category("Thickness formatting")]
        [DisplayName("Thickness attributes")]
        [Description("Defines a list of all the attributes that gets reformatted if content looks like a thickness")]
        [DefaultValue("Margin, Padding, BorderThickness, ThumbnailClipMargin")]
        public string ThicknessAttributes { get; set; }

        // Misc
        [Category("Misc")]
        [DisplayName("Beautify on saving XAML")]
        [Description("Defines whether to automatically beautify the active XAML document while saving.")]
        [DefaultValue(true)]
        public bool BeautifyOnSave { get; set; }

        [Category("Misc")]
        [DisplayName("Number of spaces to pad comments with")]
        [Description(@"Defines how many spaces a XML comment should be padded with. e.g.:
2: <!--  Hello world  -->
1: <!-- Hello world -->
0: <!--Hello world-->")]
        [DefaultValue((byte)2)]
        public byte CommentSpaces { get; set; }
    }
}