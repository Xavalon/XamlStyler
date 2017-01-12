// © Xavalon. All rights reserved.

using Xavalon.XamlStyler.Core.DocumentManipulation;

namespace Xavalon.XamlStyler.Core.Options
{
    /// <summary>
    /// Options controls how Styler works.
    /// </summary>
    public interface IStylerOptions
    {
        #region Indentation

        int IndentSize { get; set; }

        bool UseVisualStudioIndentSize { get; }

        bool IndentWithTabs { get; set; }

        #endregion Indentation

        #region Attribute formatting

        int AttributesTolerance { get; set; }

        bool KeepFirstAttributeOnSameLine { get; set; }

        string FirstLineAttributes { get; set; }

        int MaxAttributeCharactersPerLine { get; set; }

        int MaxAttributesPerLine { get; set; }

        string NoNewLineElements { get; set; }

        bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

        int AttributeIndentation { get; set; }

        bool RemoveDesignTimeReferences { get; set; }

        AttributeIndentationStyle AttributeIndentationStyle { get; set; }

        #endregion Attribute formatting

        #region Attribute Reordering

        bool EnableAttributeReordering { get; set; }

        string[] AttributeOrderingRuleGroups { get; set; }

        bool OrderAttributesByName { get; set; }

        #endregion Attribute Reordering

        #region Element formatting

        bool PutEndingBracketOnNewLine { get; set; }

        bool RemoveEndingTagOfEmptyElement { get; set; }

        bool SpaceBeforeClosingSlash { get; set; }

        LineBreakRule RootElementLineBreakRule { get; set; }

        VisualStateManagerRule ReorderVSM { get; set; }

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

        #region Thickness formatting

        ThicknessStyle ThicknessStyle { get; set; }

        string ThicknessAttributes { get; set; }

        #endregion Thickness formatting

        #region Misc

        bool BeautifyOnSave { get; set; }

        int CommentSpaces { get; set; }

        #endregion Misc

        #region Configuration

        string ConfigPath { get; set; }

        bool ResetToDefault { get; set; }

        #endregion Configuration
    }
}