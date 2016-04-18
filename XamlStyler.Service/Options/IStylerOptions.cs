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

        #region Attribute formatting

        int AttributesTolerance { get; set; }

        bool KeepFirstAttributeOnSameLine { get; set; }

        int MaxAttributeCharatersPerLine { get; set; }

        int MaxAttributesPerLine { get; set; }

        string NoNewLineElements { get; set; }

        #endregion Attribute formatting

        #region Attribute Reordering

        bool EnableAttributeReordering { get; set; }

        string[] AttributeOrderingRuleGroups { get; set; }

        bool OrderAttributesByName { get; set; }

        bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        #endregion Attribute Reordering

        #region Element formatting

        bool PutEndingBracketOnNewLine { get; set; }

        bool RemoveEndingTagOfEmptyElement { get; set; }

        bool SpaceBeforeClosingSlash { get; set; }

        LineBreakRule RootElementLineBreakRule { get; set; }

        #endregion Element formatting

        #region Element reordering

        bool ReorderGridChildren { get; set; }

        bool ReorderCanvasChildren { get; set; }

        ReorderSettersBy ReorderSetters { get; set; }

        #endregion Element reordering

        #region Markup Extension

        bool FormatMarkupExtension { get; set; }

        string NoNewLineMarkupExtensions { get; set; }

        #endregion Markup Extension

        #region Misc

        bool BeautifyOnSave { get; set; }

        byte CommentSpaces { get; set; }

        #endregion Misc
    }
}