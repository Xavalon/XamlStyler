// © Xavalon. All rights reserved.

using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Xavalon.XamlStyler.Core.DocumentManipulation;

namespace Xavalon.XamlStyler.Core.Options
{
    public class StylerOptions : IStylerOptions
    {
        private const string DefaultOptionsPath = "Xavalon.XamlStyler.Core.Options.DefaultSettings.json";

        public StylerOptions()
        {
            this.InitializeProperties();
        }

        /// <summary>
        /// Constructor that accepts an external configuration path before initializing settings.
        /// </summary>
        /// <param name="config">Path to external configuration file.</param>
        public StylerOptions(string config = "")
        {
            if (!String.IsNullOrWhiteSpace(config) && File.Exists(config))
            {
                this.ConfigPath = config;
            }

            this.InitializeProperties();
        }

        /// <summary>
        /// JSON Constructor required to prevent an infinite loop during deserialization.
        /// </summary>
        /// <param name="isJsonConstructor">Dummy parameter to differentiate from default constructor.</param>
        [JsonConstructor]
        public StylerOptions(bool isJsonConstructor = true) { }

        // Indentation
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(2)]
        [JsonProperty("IndentSize", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Browsable(false)]
        public int IndentSize { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Browsable(false)]
        [JsonIgnore]
        public bool IndentWithTabs { get; set; }

        // Attribute formatting
        [Category("Attribute Formatting")]
        [DisplayName("Attribute tolerance")]
        [JsonProperty("AttributesTolerance", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines the attribute number tolerance before XamlStyler starts to break attributes into new lines. 0 = no tolerance.\r\n\r\ne.g., when this setting is 2\r\n\r\nBEFORE BEAUTIFY:\r\n<TextBlock x:Name=\"m_sample\"\r\n    Text=\"asdf\" />\r\n\r\nAFTER BEAUTIFY:\r\n<TextBlock x:Name=\"m_sample\" Text=\"asdf\" />\r\n\r\nDefault Value: 2")]
        [DefaultValue(2)]
        public int AttributesTolerance { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Position first attribute on same line as start tag")]
        [JsonProperty("KeepFirstAttributeOnSameLine", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether the first line of attribute(s) shall appear on the same line as the element's start tag.\r\n\r\ne.g., when this setting is true\r\n\r\nBEFORE BEAUTIFY:\r\n\"<element a='xyz' b='xyz'>  </element>\"\n\r\nAFTER BEAUTIFY:\r\n\"<element a='xyz'\r\n        b='xyz'>\r\n</element>\"\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool KeepFirstAttributeOnSameLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Max attribute characters per line")]
        [JsonProperty("MaxAttributeCharatersPerLine", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines the maximum character length (not including indentation characters) of attributes an element can have on each line after the start tag. A value less than 1 meaning no limit.\r\n\r\nDefault Value: 0")]
        [DefaultValue(0)]
        public int MaxAttributeCharatersPerLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Max attributes per line")]
        [JsonProperty("MaxAttributesPerLine", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines the maximum number of attributes an element can have on each line after the start tag. 0 = no limit.\r\n\r\nDefault Value: 1")]
        [DefaultValue(1)]
        public int MaxAttributesPerLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Elements no line break between attributes")]
        [JsonProperty("NewlineExemptionElements", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines a list of elements whose attributes shall not be broken into lines.\r\n\r\nDefault Value: RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Setter")]
        [DefaultValue("RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter")]
        public string NoNewLineElements { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Put Rule Groups on separate Lines")]
        [JsonProperty("SeparateByGroups", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Put attributes belonging to different rule groups on different lines and keep identical groups on same line if possible\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Attribute indentation")]
        [JsonProperty("AttributeIndentation", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines the number of spaces that attributes are indented, on elements with more than one line of attributes.\r\n\r\n0 = align with first attribute.\r\n\r\nDefault Value: 0")]
        [DefaultValue(0)]
        public int AttributeIndentation { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Attribute indentation style")]
        [JsonProperty("AttributeIndentationStyle", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines how attributes are indented after Element indentation has been applied.\r\n\r\nMixed = Tabs (if using tabs) then spaces.\r\nSpaces = Always use spaces.\r\n\r\nDefault Value: Spaces")]
        [DefaultValue(AttributeIndentationStyle.Spaces)]
        public AttributeIndentationStyle AttributeIndentationStyle { get; set; }

        // Attribute Reordering
        [Category("Attribute Reordering")]
        [DisplayName("Enable Attribute Reordering")]
        [JsonProperty("EnableAttributeReordering", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("If this is disabled, attributes will not be reordered in any way.\r\n\r\nDefault Value: True")]
        [DefaultValue(true)]
        public bool EnableAttributeReordering { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("Attribute Ordering Rule Groups")]
        [JsonProperty("AttributeOrderingRuleGroups", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines attribute ordering rule groups. Each string element is one group. Use ',' to separate more than one attribute.'DOS' wildcards are allowed. Attributes listed in earlier groups takes precedence over later groups. Attributes listed earlier in same group takes precedence over the ones listed later.")]
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
        [DisplayName("First Line Attributes")]
        [JsonProperty("FirstLineAttributes", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines a list of attributes which should always appear on the same line as the element's start tag. Attribute reordering must be enabled for this setting to take effect.\r\n\r\nDefault Value: None")]
        [DefaultValue("")]
        public string FirstLineAttributes { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("Order Attributes by name")]
        [JsonProperty("OrderAttributesByName", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Order Attributes by name if order is not determined by Rules.\r\n\r\nDefault Value: True")]
        [DefaultValue(true)]
        public bool OrderAttributesByName { get; set; }

        // Element formatting
        [Category("Element Formatting")]
        [DisplayName("Put ending bracket on new line")]
        [JsonProperty("PutEndingBracketOnNewLine", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to put \">\" or \"/>\" on a new line.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool PutEndingBracketOnNewLine { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Remove ending tag of empty element")]
        [JsonProperty("RemoveEndingTagOfEmptyElement", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to remove the ending tag of an empty element.\r\n\r\ne.g., when this setting is true\r\n\r\nBEFORE BEAUTIFY:\r\n\"<element>  </element>\"\r\n\r\nAFTER BEAUTIFY:\r\n\"<element />\"\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool RemoveEndingTagOfEmptyElement { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Space before closing slash in self closing element")]
        [JsonProperty("SpaceBeforeClosingSlash", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to have a space before slash in self closing elements\r\n\r\ne.g., when:\r\ntrue: <br />\r\nfalse: <br/>\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool SpaceBeforeClosingSlash { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Root element line breaks between attributes")]
        [JsonProperty("RootElementLineBreakRule", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines if attributes of the document root element are broken into separate lines or not.\r\n\r\nDefault Value: Default (use same rules as other elements)")]
        [DefaultValue(LineBreakRule.Default)]
        public LineBreakRule RootElementLineBreakRule { get; set; }

        // Element reordering
        [Category("Element Reordering")]
        [DisplayName("Reorder Visual State Manager")]
        [JsonProperty("ReorderVSM", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to reorder the visual state manager. When set to first or last, the visual state manager will be moved to the first or last child element in its parent, respectively, otherwise it will not be moved.\r\n\r\nDefault Value: Last")]
        [DefaultValue(VisualStateManagerRule.Last)]
        public VisualStateManagerRule ReorderVSM { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder Grid panel children by row/column")]
        [JsonProperty("ReorderGridChildren", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to reorder the children of a Grid by row/column. When this is true, children will be reordered in an ascending fashion by looking at their attached Grid properties: first by Grid.Row, then by Grid.Column.\r\n\r\nDefault Value: True")]
        [DefaultValue(true)]
        public bool ReorderGridChildren { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder Canvas panel children by left/top/right/bottom")]
        [JsonProperty("ReorderCanvasChildren", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to reorder the children of a Canvas by left/top/right/bottom.  When this is true, children will be reordered in an ascending fashion by looking at their attached Canvas properties: first by Canvas.Left, then by Canvas.Top, then by Canvas.Right, then by Canvas.Bottom.\r\n\r\nDefault Value: True")]
        [DefaultValue(true)]
        public bool ReorderCanvasChildren { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder Setters by")]
        [JsonProperty("ReorderSetters", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to reorder 'Setter' elements in style/trigger elements. When this is set, children will be reordered in an ascending fashion by looking at their Property and/or TargetName properties.\r\n\r\nDefault Value: None")]
        [DefaultValue(ReorderSettersBy.None)]
        public ReorderSettersBy ReorderSetters { get; set; }

        // Markup Extension
        [Category("Markup Extension")]
        [DisplayName("Enable Markup Extension Formatting")]
        [JsonProperty("FormatMarkupExtension", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to format markup extensions (attributes containing '{}'). When this setting is true, attributes with markup extensions will always be put on a new line, UNLESS the element is under AttributesTolerance or one of the NoNewLineElements.\r\n\r\nDefault Value: True")]
        [DefaultValue(true)]
        public bool FormatMarkupExtension { get; set; }

        [Category("Markup Extension")]
        [DisplayName("Keep Markup Extensions of these types on one line")]
        [JsonProperty("NoNewLineMarkupExtensions", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines a comma separated list of Markup Extensions that are always kept on one line.\r\n\r\nDefault Value: x:Bind")]
        [DefaultValue("x:Bind")]
        public string NoNewLineMarkupExtensions { get; set; }

        // Thickness formatting
        [Category("Thickness formatting")]
        [DisplayName("Thickness style")]
        [JsonProperty("ThicknessSeparator", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines how Thickness properties like Margin, Padding etc. should be formatted.\r\n\r\nDefault Value: None")]
        [DefaultValue(ThicknessStyle.None)]
        public ThicknessStyle ThicknessStyle { get; set; }

        [Category("Thickness formatting")]
        [DisplayName("Thickness attributes")]
        [JsonProperty("ThicknessAttributes", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines a list of all the attributes that gets reformatted if content looks like a thickness\r\n\r\nDefault Value: Margin, Padding, BorderThickness, ThumbnailClipMargin")]
        [DefaultValue("Margin, Padding, BorderThickness, ThumbnailClipMargin")]
        public string ThicknessAttributes { get; set; }

        // Misc
        [Category("Misc")]
        [DisplayName("Beautify on saving XAML")]
        [JsonProperty("FormatOnSave", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to automatically beautify the active XAML document while saving.\r\n\r\nDefault Value: True")]
        [DefaultValue(true)]
        public bool BeautifyOnSave { get; set; }

        [Category("Misc")]
        [DisplayName("Number of spaces to pad comments with")]
        [JsonProperty("CommentPadding", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines how many spaces a XML comment should be padded with. e.g.:\r\n2: <!--  Hello world  -->\r\n1: <!-- Hello world -->\r\n0: <!--Hello world-->.\r\n\r\nDefault Value: 2")]
        [DefaultValue(2)]
        public int CommentSpaces { get; set; }

        // Configuration
        private bool resetToDefault;

        [Category("XAML Styler Configuration")]
        [RefreshProperties(RefreshProperties.All)]
        [DisplayName("Reset to Default")]
        [Description("When set to true, all XAML Styler settings will be reset to their defaults.")]
        [DefaultValue(false)]
        [JsonIgnore]
        public bool ResetToDefault
        {
            get { return this.resetToDefault; }
            set
            {
                this.resetToDefault = value;

                if (this.resetToDefault)
                {
                    this.ConfigPath = String.Empty;
                    this.InitializeProperties();
                }
            }
        }

        private string configPath = String.Empty;

        [Category("XAML Styler Configuration")]
        [RefreshProperties(RefreshProperties.All)]
        [DisplayName("External Configuration File")]
        [Description("Defines location of external XAML Styler configuration file. Specifying an external configuration file allows you to easily point multiple instances to a shared configuration. The configuration path can be local or network-based. Invalid configurations will be ignored.\r\n\r\nDefault Value: N/A")]
        [DefaultValue("")]
        [JsonIgnore]
        public string ConfigPath
        {
            get { return this.configPath; }
            set
            {
                this.configPath = value;

                if(!String.IsNullOrEmpty(value))
                {
                    this.TryLoadExternalConfiguration();
                }
            }
        }

        private void InitializeProperties()
        {
            if (!this.TryLoadExternalConfiguration())
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(StylerOptions.DefaultOptionsPath))
                using (StreamReader reader = new StreamReader(stream))
                {
                    this.LoadConfiguration(reader.ReadToEnd());
                }
            }
        }

        private bool TryLoadExternalConfiguration()
        {
            if (String.IsNullOrWhiteSpace(this.ConfigPath) || !File.Exists(this.ConfigPath))
            {
                return false;
            }

            return this.LoadConfiguration(File.ReadAllText(this.ConfigPath));
        }

        private bool LoadConfiguration(string config)
        {
            try
            {
                StylerOptions configOptions = JsonConvert.DeserializeObject<StylerOptions>(config);

                if (configOptions == null)
                {
                    this.LoadFallbackConfiguration();
                }
                else
                {
                    foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(this))
                    {
                        // Cannot set Config Path from External Configuration.
                        if (propertyDescriptor.Name.Equals(nameof(this.ConfigPath)))
                        {
                            continue;
                        }

                        propertyDescriptor.SetValue(this, propertyDescriptor.GetValue(configOptions));
                    }
                }
            }
            catch (Exception)
            {
                this.LoadFallbackConfiguration();
                return false;
            }

            return true;
        }

        private void LoadFallbackConfiguration()
        {
            // Initialize all properties with "DefaultValueAttrbute" to their default value
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(this))
            {
                // Set default value if DefaultValueAttribute is present
                DefaultValueAttribute attribute
                    = propertyDescriptor.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

                if (attribute != null)
                {
                    propertyDescriptor.SetValue(this, attribute.Value);
                }
            }
        }
    }
}