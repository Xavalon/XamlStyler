using System.ComponentModel;

namespace XamlStyler.Core.Options
{
    /// <summary>
    /// Options controls how Styler works
    /// </summary>
    public interface IStylerOptions
    {
        #region Indentation

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        int IndentSize { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        bool IndentWithTabs { get; set; }

        #endregion Indentation

        #region Attribute Ordering Rule Groups
        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#10 Order attributes by name")]
        [Description("Enable sorting of attributes by name")]
        [DefaultValue("True")]
        bool OrderAttributesByName { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#7 Alignment layout group")]
        [Description(
            "Defines ordering rule of alignment layout attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later."
            )]
        [DefaultValue(
            "HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex"
            )]
        string AttributeOrderAlignmentLayout { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#5 Attached layout group")]
        [Description(
            "Defines ordering rule of attached layout attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later."
            )]
        [DefaultValue(
            "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom"
            )]
        string AttributeOrderAttachedLayout { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#9 Blend related group")]
        [Description(
            "Defines ordering rule of blend related attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later."
            )]
        [DefaultValue("mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText")]
        string AttributeOrderBlendRelated { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#1 Class definition group")]
        [Description(
            "Defines ordering rule of class definition attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later."
            )]
        [DefaultValue("x:Class")]
        string AttributeOrderClass { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#6 Core layout group")]
        [Description(
            "Defines ordering rule of core layout attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later."
            )]
        [DefaultValue("Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight, Margin")]
        string AttributeOrderCoreLayout { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#3 Element key group")]
        [Description(
            "Defines ordering rule of element key.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later."
            )]
        [DefaultValue("Key, x:Key, Uid, x:Uid")]
        string AttributeOrderKey { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#4 Element name group")]
        [Description(
            "Defines ordering rule of element name.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later."
            )]
        [DefaultValue("Name, x:Name, Title")]
        string AttributeOrderName { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#8 Miscellaneous attributes group")]
        [Description(
            "Defines ordering rule of miscellaneous attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.\r\n**Attributes not listed in any ordring rule groups, are implicitly appended to this group in alphabetic order."
            )]
        [DefaultValue("PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint")]
        string AttributeOrderOthers { get; set; }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#2 WPF Namespaces group")]
        [Description(
            "Defines ordering rule of wpf namespaces.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later."
            )]
        [DefaultValue("xmlns, xmlns:x")]
        string AttributeOrderWpfNamespace { get; set; }

        #endregion Attribute Ordering Rule Groups

        #region Markup Extension

        [Category("Markup Extension")]
        [DisplayName("Enable Markup Extension Formatting")]
        [Description(
            "Defines whether to format markup extension.\r\nDefalut Value: true\r\nWhen this setting is true, attributes with markup extension will always be put on a new line, UNLESS the element is under AttributesTolerance or one of the NoNewLineElements."
            )]
        [DefaultValue(true)]
        bool FormatMarkupExtension { get; set; }

        #endregion Markup Extension

        #region New Line

        [Category("New Line")]
        [DisplayName("Attribute tolerance")]
        [Description(
            "Defines the attribute number tolerance before XamlStyler starts to break attributes into new lines. A value less than 1 meaning no tolerance.\r\ne.g., when this setting is 2\r\n\r\nBEFORE BEAUTIFY:\r\n<TextBlock x:Name=\"m_sample\"\r\n    Text=\"asdf\" />\r\n\r\nAFTER BEAUTIFY:\r\n<TextBlock x:Name=\"m_sample\" Text=\"asdf\" />\r\nDefault Value: 2"
            )]
        [DefaultValue(2)]
        int AttributesTolerance { get; set; }

        [Category("New Line")]
        [DisplayName("Position first attribute on same line as start tag")]
        [Description(
            "Defines whether the first line of attribute(s) shall appear on the same line as the element's start tag.\r\ne.g., when this setting is true\r\n\r\nBEFORE BEAUTIFY:\r\n\"<element a='xyz' b='xyz'>  </element>\"\r\n\r\nAFTER BEAUTIFY:\r\n\"<element a='xyz'\r\n        b='xyz'>\r\n</element>\"\r\n\r\nDefault Value: true"
            )]
        [DefaultValue(true)]
        bool KeepFirstAttributeOnSameLine { get; set; }

        [Category("New Line")]
        [DisplayName("Max attribute characters per line")]
        [Description(
            "Defines the maximum charater length (not including indentation characters) of attributes an element can have on each line after the start tag. A value less than 1 meaning no limit.\r\nDefault Value: 0"
            )]
        [DefaultValue(0)]
        int MaxAttributeCharatersPerLine { get; set; }

        [Category("New Line")]
        [DisplayName("Max attribute number per line")]
        [Description(
            "Defines the maximum number of attributes an element can have on each line after the start tag. A value less than 1 meaning no limit.\r\nDefault Value: 1"
            )]
        [DefaultValue(1)]
        int MaxAttributesPerLine { get; set; }

        [Category("New Line")]
        [DisplayName("Elements no line break between attributes")]
        [Description(
            "Defines a list of elements whose attributes shall not be broken into lines.\r\nDefault Value: RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Setter"
            )]
        [DefaultValue(
            "RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter"
            )]
        string NoNewLineElements { get; set; }

        [Category("New Line")]
        [DisplayName("Put attributes order rule groups on separate lines")]
        [Description(
            "Put attributes belonging to different order rules on different lines (keep identical order rules on same line if possible)"
            )]
        [DefaultValue(false)]
        bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        [Category("New Line")]
        [DisplayName("Put ending bracket on new line")]
        [Description("Defines whether to put \">\" or \"/>\" on a new line.\r\nDefault Value: false")]
        [DefaultValue(false)]
        bool PutEndingBracketOnNewLine { get; set; }

        [Category("New Line")]
        [DisplayName("Remove ending tag of empty element")]
        [Description(
            "Defines whether to remove the ending tag of an empty element.\r\ne.g., when this setting is true\r\n\r\nBEFORE BEAUTIFY:\r\n\"<element>  </element>\"\r\n\r\nAFTER BEAUTIFY:\r\n\"<element />\"\r\n\r\nDefault Value: true"
            )]
        [DefaultValue(true)]
        bool RemoveEndingTagOfEmptyElement { get; set; }

        [Category("New Line")]
        [DisplayName("Root element line breaks between attributes")]
        [Description(
            "Defines if attributes of the document root element are broken into separate lines or not.\r\nDefault = use same rules as other elements"
            )]
        [DefaultValue(LineBreakRule.Default)]
        LineBreakRule RootElementLineBreakRule { get; set; }

        #endregion New Line

        #region Misc

        [Category("Misc")]
        [DisplayName("Beautify on saving xaml")]
        [Description("Defines whether to automatically beautify the active xaml document while saving.")]
        [DefaultValue(true)]
        bool BeautifyOnSave { get; set; }

        #endregion Misc

        #region Content Order

        [Category("Content Order")]
        [DisplayName("Reorder Grid panel children by row/column")]
        [Description("Defines whether to reorder the children of a Grid by row/column.  When this is true, children will be reordered in an ascending fashion by looking at their attached Grid properties: first by Grid.Row, then by Grid.Column.")]
        [DefaultValue(true)]
        bool ReorderGridChildren { get; set; }

        [Category("Content Order")]
        [DisplayName("Reorder Canvas panel children by left/top/right/bottom")]
        [Description("Defines whether to reorder the children of a Canvas by left/top/right/bottom.  When this is true, children will be reordered in an ascending fashion by looking at their attached Canvas properties: first by Canvas.Left, then by Canvas.Top, then by Canvas.Right, then by Canvas.Bottom.")]
        [DefaultValue(true)]
        bool ReorderCanvasChildren { get; set; }

        [Category("Content Order")]
        [DisplayName("Reorder Setters by")]
        [Description("Defines whether to reorder 'Setter' elements in style/trigger elements. When this is set, children will be reordered in an ascending fashion by looking at their Property and/or TargetName properties")]
        [DefaultValue(ReorderSettersBy.None)]
        ReorderSettersBy ReorderSetters { get; set; }

        #endregion
    }
}