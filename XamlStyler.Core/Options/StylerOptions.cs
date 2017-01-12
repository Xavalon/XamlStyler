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
        private const int FallbackIndentSize = 2;

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
        [DefaultValue(-1)]
        [JsonProperty("IndentSize", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Browsable(false)]
        public int IndentSize { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        [Browsable(false)]
        [JsonIgnore]
        public bool UseVisualStudioIndentSize { get; private set; } = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Browsable(false)]
        [JsonIgnore]
        public bool IndentWithTabs { get; set; }

        // Attribute formatting
        [Category("Attribute Formatting")]
        [DisplayName("Attribute tolerance")]
        [JsonProperty("AttributesTolerance", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines the maximum number of attributes allowed on a single line. If the number of attributes exceeds this value, XAML Styler will break the attributes up across multiple lines. A value of less than 1 means always break up the attributes.\r\n\r\nDefault Value: 2")]
        [DefaultValue(2)]
        public int AttributesTolerance { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Keep first attribute on same line")]
        [JsonProperty("KeepFirstAttributeOnSameLine", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether the first line of attribute(s) should appear on the same line as the element's start tag.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool KeepFirstAttributeOnSameLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Max attribute characters per line")]
        [JsonProperty("MaxAttributeCharactersPerLine", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines the maximum character length of attributes an element can have on each line after the start tag (not including indentation characters). A value of less than 1 means no limit. Note: This setting only takes effect if Max Attributes Per Line is greater than 1 and Attribute Tolerance is greater than 2.\r\n\r\nDefault Value: 0")]
        [DefaultValue(0)]
        public int MaxAttributeCharactersPerLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Max attributes per line")]
        [JsonProperty("MaxAttributesPerLine", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines the maximum number of attributes an element can have on each line after the start tag if the number of attributes exceeds the attribute tolerance. A value of less than 1 means no limit.\r\n\r\nDefault Value: 1")]
        [DefaultValue(1)]
        public int MaxAttributesPerLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Newline exemption elements")]
        [JsonProperty("NewlineExemptionElements", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines a list of elements whose attributes should not be broken across lines.\r\n\r\nDefault Value: RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Setter")]
        [DefaultValue("RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter")]
        public string NoNewLineElements { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Separate by groups")]
        [JsonProperty("SeparateByGroups", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether attributes belonging to different rule groups should be put on separate lines, while, if possible, keeping attributes in the same group on the same line.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Attribute indentation")]
        [JsonProperty("AttributeIndentation", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines the number of spaces that attributes are indented on elements with more than one line of attributes. A value of 0 will align indentation with the first attribute.\r\n\r\nDefault Value: 0")]
        [DefaultValue(0)]
        public int AttributeIndentation { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Attribute indentation style")]
        [JsonProperty("AttributeIndentationStyle", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines how attributes are indented.\r\n\r\nDefault Value: Spaces")]
        [DefaultValue(AttributeIndentationStyle.Spaces)]
        public AttributeIndentationStyle AttributeIndentationStyle { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Remove design-time references")]
        [JsonProperty("RemoveDesignTimeReferences", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether design-time references automatically added to new pages and controls should be removed.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool RemoveDesignTimeReferences { get; set; }

        // Attribute Reordering
        [Category("Attribute Reordering")]
        [DisplayName("Enable Attribute Reordering")]
        [JsonProperty("EnableAttributeReordering", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether attributes should be reordered. If false, attributes will not be reordered in any way.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool EnableAttributeReordering { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("Attribute ordering rule groups")]
        [JsonProperty("AttributeOrderingRuleGroups", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines attribute ordering rule groups. Each string element is one group. Use ',' as a delimiter between attributes. 'DOS' wildcards are allowed. XAML Styler will order attributes in groups from top to bottom, and within groups left to right.")]
        [DefaultValue(new[]
        {
            "x:Class",
            "xmlns, xmlns:x",
            "xmlns:*",
            "x:Key, Key, x:Name, Name, x:Uid, Uid, Title",
            "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
            "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight",
            "Margin, Padding, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex",
            "*:*, *",
            "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint",
            "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText",
            //Storyboards, fixes #30
            "Storyboard.*, From, To, Duration",
        })]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] AttributeOrderingRuleGroups { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("First-line attributes")]
        [JsonProperty("FirstLineAttributes", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines a list of attributes which should always appear on the same line as the element's start tag. Attribute reordering must be enabled for this setting to take effect.\r\n\r\nDefault Value: None")]
        [DefaultValue("")]
        public string FirstLineAttributes { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("Order attributes by name")]
        [JsonProperty("OrderAttributesByName", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether attributes should be ordered by name if not determined by a rule.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool OrderAttributesByName { get; set; }

        // Element formatting
        [Category("Element Formatting")]
        [DisplayName("Put ending brackets on new line")]
        [JsonProperty("PutEndingBracketOnNewLine", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to put ending brackets on a new line.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool PutEndingBracketOnNewLine { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Remove ending tag of empty elements")]
        [JsonProperty("RemoveEndingTagOfEmptyElement", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to remove the end tag of an empty element.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool RemoveEndingTagOfEmptyElement { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Space before ending slash in self-closing elements")]
        [JsonProperty("SpaceBeforeClosingSlash", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether there should be a space before the slash in ending brackets for self-closing elements.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool SpaceBeforeClosingSlash { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Root element line breaks between attributes")]
        [JsonProperty("RootElementLineBreakRule", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether attributes of the document root element are broken into multiple lines.\r\n\r\nDefault Value: Default (use same rules as other elements)")]
        [DefaultValue(LineBreakRule.Default)]
        public LineBreakRule RootElementLineBreakRule { get; set; }

        // Element reordering
        [Category("Element Reordering")]
        [DisplayName("Reorder visual state manager")]
        [JsonProperty("ReorderVSM", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to reorder the visual state manager. When set to first or last, the visual state manager will be moved to the first or last child element in its parent, respectively, otherwise it will not be moved.\r\n\r\nDefault Value: Last")]
        [DefaultValue(VisualStateManagerRule.Last)]
        public VisualStateManagerRule ReorderVSM { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder grid panel children")]
        [JsonProperty("ReorderGridChildren", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to reorder the children of a Grid by row/column. When true, children will be reordered in an ascending fashion by looking first at Grid.Row, then by Grid.Column.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool ReorderGridChildren { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder canvas panel children")]
        [JsonProperty("ReorderCanvasChildren", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to reorder the children of a Canvas by left/top/right/bottom. When true, children will be reordered in an ascending fashion by first at Canvas.Left, then by Canvas.Top, Canvas.Right, and finally, Canvas.Bottom.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool ReorderCanvasChildren { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder setters")]
        [JsonProperty("ReorderSetters", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to reorder 'Setter' elements in style/trigger elements. When this is set, children will be reordered in an ascending fashion by looking at their Property and/or TargetName properties.\r\n\r\nDefault Value: None")]
        [DefaultValue(ReorderSettersBy.None)]
        public ReorderSettersBy ReorderSetters { get; set; }

        // Markup Extension
        [Category("Markup Extension")]
        [DisplayName("Enable markup extension formatting")]
        [JsonProperty("FormatMarkupExtension", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to format Markup Extensions (attributes containing '{}'). When true, attributes with markup extensions will always be put on a new line, unless the element is under the attribute tolerance or one of the specified elements is in the list of elements with no line breaks between attributes.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool FormatMarkupExtension { get; set; }

        [Category("Markup Extension")]
        [DisplayName("Keep markup extensions of these types on one line")]
        [JsonProperty("NoNewLineMarkupExtensions", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines a comma-separated list of Markup Extensions that are always kept on a single line\r\n\r\nDefault Value: x:Bind, Binding")]
        [DefaultValue("x:Bind, Binding")]
        public string NoNewLineMarkupExtensions { get; set; }

        // Thickness formatting
        [Category("Thickness formatting")]
        [DisplayName("Thickness separator")]
        [JsonProperty("ThicknessSeparator", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines how thickness attributes (i.e., margin, padding, etc.) should be formatted.\r\n\r\nDefault Value: Comma")]
        [DefaultValue(ThicknessStyle.Comma)]
        public ThicknessStyle ThicknessStyle { get; set; }

        [Category("Thickness formatting")]
        [DisplayName("Thickness attributes")]
        [JsonProperty("ThicknessAttributes", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines a list of attributes that get reformatted if content appears to be a thickness.\r\n\r\nDefault Value: Margin, Padding, BorderThickness, ThumbnailClipMargin")]
        [DefaultValue("Margin, Padding, BorderThickness, ThumbnailClipMargin")]
        public string ThicknessAttributes { get; set; }

        // Misc
        [Category("Misc")]
        [DisplayName("Format XAML on save")]
        [JsonProperty("FormatOnSave", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to automatically format the active XAML document while saving.\r\n\r\nDefault Value: false")]
        [DefaultValue(true)]
        public bool BeautifyOnSave { get; set; }

        [Category("Misc")]
        [DisplayName("Comment padding")]
        [JsonProperty("CommentPadding", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Determines the number of spaces a XAML comment should be padded with.\r\n\r\nDefault Value: 2")]
        [DefaultValue(2)]
        public int CommentSpaces { get; set; }

        // Configuration
        private bool resetToDefault;

        [Category("XAML Styler Configuration")]
        [RefreshProperties(RefreshProperties.All)]
        [DisplayName("Reset to default")]
        [Description("When set to true, all XAML Styler settings will be reset to their defaults.\r\n\r\nDefault Value: false")]
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
        [DisplayName("External configuration file")]
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

        /// <summary>
        /// Creates a clone from the current instance.
        /// </summary>
        /// <returns>A clone from the current instance.</returns>
        public IStylerOptions Clone()
        {
            var jsonStylerOptions = JsonConvert.SerializeObject(this);

            return JsonConvert.DeserializeObject<StylerOptions>(jsonStylerOptions);
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

                        // If a valid IndentSize is specified in configuration, do not load VS settings.
                        if (propertyDescriptor.Name.Equals(nameof(this.IndentSize)))
                        {
                            int indentSize;
                            try
                            {
                                indentSize = Convert.ToInt32(propertyDescriptor.GetValue(configOptions));
                            }
                            catch (Exception)
                            {
                                indentSize = -1;
                            }
                            

                            // Cannot specify MissingMemberHandling for a single property, so relying on JSON default
                            // value to detect missing member, and setting default on detection.
                            if (indentSize > 0)
                            {
                                this.IndentSize = indentSize;
                                this.UseVisualStudioIndentSize = false;
                            }
                            else
                            {
                                this.IndentSize = StylerOptions.FallbackIndentSize;
                            }
                        }
                        else
                        {
                            propertyDescriptor.SetValue(this, propertyDescriptor.GetValue(configOptions));
                        }
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