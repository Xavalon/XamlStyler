using System.ComponentModel;

namespace XamlStyler.Core.Options
{
    /// <summary>
    /// Options controls how Styler works
    /// </summary>
    public interface IStylerOptions
    {
        #region Indentation

        int IndentSize { get; set; }

        bool IndentWithTabs { get; set; }

        #endregion Indentation

        #region Attribute Ordering Rule Groups
        bool OrderAttributesByName { get; set; }

        string AttributeOrderAlignmentLayout { get; set; }

        string AttributeOrderAttachedLayout { get; set; }

        string AttributeOrderBlendRelated { get; set; }

        string AttributeOrderClass { get; set; }

        string AttributeOrderCoreLayout { get; set; }

        string AttributeOrderKey { get; set; }

        string AttributeOrderName { get; set; }

        string AttributeOrderOthers { get; set; }

        string AttributeOrderWpfNamespace { get; set; }

        #endregion Attribute Ordering Rule Groups

        #region Markup Extension

        bool FormatMarkupExtension { get; set; }

        #endregion Markup Extension

        #region New Line
        bool KeepxBindOnSameLine { get; set; }

        bool KeepBindingsOnSameLine { get; set; }

        int AttributesTolerance { get; set; }

        bool KeepFirstAttributeOnSameLine { get; set; }

        int MaxAttributeCharatersPerLine { get; set; }

        int MaxAttributesPerLine { get; set; }

        string NoNewLineElements { get; set; }

        bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        bool PutEndingBracketOnNewLine { get; set; }

        bool RemoveEndingTagOfEmptyElement { get; set; }

        bool SpaceBeforeClosingSlash { get; set; }

        LineBreakRule RootElementLineBreakRule { get; set; }

        #endregion New Line

        #region Misc

        bool BeautifyOnSave { get; set; }

        #endregion Misc

        #region Content Order

        bool ReorderGridChildren { get; set; }

        bool ReorderCanvasChildren { get; set; }

        ReorderSettersBy ReorderSetters { get; set; }

        #endregion
    }
}