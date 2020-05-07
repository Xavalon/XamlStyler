using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Controls.FileSystem;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Application.UI.Options.OptionsDialog.SimpleOptions;
using JetBrains.DataFlow;
using JetBrains.IDE.UI;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Extensions.Properties;
using JetBrains.IDE.UI.Extensions.Validation;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Resources;
using JetBrains.Rider.Model.UIAutomation;
using JetBrains.Util;
using Xavalon.XamlStyler.Core.Options;

namespace ReSharperPlugin.XamlStyler.dotUltimate.Options
{
    [OptionsPage(PID, PageTitle, typeof(FeaturesEnvironmentOptionsThemedIcons.FormattingStyle), ParentId = ToolsPage.PID)]
    public class XamlStylerOptionsPage : BeSimpleOptionsPage
    {
        private const string PID = nameof(XamlStylerOptionsPage);
        private const string PageTitle = "XAML Styler";

        private readonly IconHostBase _iconHost;
        
        public XamlStylerOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            [NotNull] IconHostBase iconHost,
            [NotNull] ICommonFileDialogs commonFileDialogs)
            : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
        {
            _iconHost = iconHost;
            
#if RIDER
            // General
            AddHeader("General");
            AddBoolOption((XamlStylerSettings x) => x.FormatOnSave, "Format XAML on save");
#endif

            // Indentation
            AddHeader("Indentation");
            var indentSizeOption = AddSpinner((XamlStylerSettings x) => x.IndentSize, "Indent size:");
            using (Indent())
            {
                AddBoolOption((XamlStylerSettings x) => x.UseIdeIndentSize, "Use IDE value");
            }
            
            AddBinding(indentSizeOption, BindingStyle.IsEnabledProperty,
                (XamlStylerSettings x) => x.UseIdeIndentSize,
                x => !(bool)x);
            
            var indentWithTabsOption = AddBoolOption((XamlStylerSettings x) => x.IndentWithTabs, "Indent with tabs");
            using (Indent())
            {
                AddBoolOption((XamlStylerSettings x) => x.UseIdeIndentWithTabs, "Use IDE value");
            }
            
            AddBinding(indentWithTabsOption, BindingStyle.IsEnabledProperty,
                (XamlStylerSettings x) => x.UseIdeIndentWithTabs,
                x => !(bool)x);
            
            // Attribute formatting
            AddHeader("Attribute formatting");
            AddSpinner((XamlStylerSettings x) => x.AttributesTolerance, "Attribute tolerance:");
            AddBoolOption((XamlStylerSettings x) => x.KeepFirstAttributeOnSameLine, "Keep first attribute on same line");
            AddSpinner((XamlStylerSettings x) => x.MaxAttributeCharactersPerLine, "Max attribute characters per line:");
            AddSpinner((XamlStylerSettings x) => x.MaxAttributesPerLine, "Max attributes per line:");
            AddTextBox((XamlStylerSettings x) => x.NoNewLineElements, "Newline exemption elements:");
            AddBoolOption((XamlStylerSettings x) => x.PutAttributeOrderRuleGroupsOnSeparateLines, "Separate by groups");
            AddSpinner((XamlStylerSettings x) => x.AttributeIndentation, "Attribute indentation:");
            AddComboEnum((XamlStylerSettings x) => x.AttributeIndentationStyle, "Attribute indentation style:");
            AddBoolOption((XamlStylerSettings x) => x.RemoveDesignTimeReferences, "Remove design-time references");
            
            // Attribute Reordering
            AddHeader("Attribute reordering");
            AddBoolOption((XamlStylerSettings x) => x.EnableAttributeReordering, "Enable attribute reordering");

            var attributeOrderingRuleGroupsOption = AddListControl((XamlStylerSettings x) => x.AttributeOrderingRuleGroups, new StylerOptions().AttributeOrderingRuleGroups, "Attribute ordering rule groups");
            
            var firstLineAttributesOption = AddTextBox((XamlStylerSettings x) => x.FirstLineAttributes, "First-line attributes:");
            var orderAttributesByNameOption = AddBoolOption((XamlStylerSettings x) => x.OrderAttributesByName, "Order attributes by name");
            
            AddBinding(attributeOrderingRuleGroupsOption, BindingStyle.IsEnabledProperty,
                (XamlStylerSettings x) => x.EnableAttributeReordering,
                x => (bool)x);
            
            AddBinding(firstLineAttributesOption, BindingStyle.IsEnabledProperty,
                (XamlStylerSettings x) => x.EnableAttributeReordering,
                x => (bool)x);
            
            AddBinding(orderAttributesByNameOption, BindingStyle.IsEnabledProperty,
                (XamlStylerSettings x) => x.EnableAttributeReordering,
                x => (bool)x);
            
            // Element formatting
            AddHeader("Element formatting");
            AddBoolOption((XamlStylerSettings x) => x.PutEndingBracketOnNewLine, "Put ending brackets on new line");
            AddBoolOption((XamlStylerSettings x) => x.RemoveEndingTagOfEmptyElement, "Remove ending tag of empty element");
            AddBoolOption((XamlStylerSettings x) => x.SpaceBeforeClosingSlash, "Space before ending slash in self-closing elements");
            AddComboEnum((XamlStylerSettings x) => x.RootElementLineBreakRule, "Root element line breaks between attributes:");
            
            // Element reordering
            AddHeader("Element reordering");
            AddComboEnum((XamlStylerSettings x) => x.ReorderVSM, "Reorder visual state manager:");
            AddBoolOption((XamlStylerSettings x) => x.ReorderGridChildren, "Reorder grid panel children");
            AddBoolOption((XamlStylerSettings x) => x.ReorderCanvasChildren, "Reorder canvas panel children");
            AddComboEnum((XamlStylerSettings x) => x.ReorderSetters, "Reorder setters:");

            // Markup extension
            AddHeader("Markup extension");
            AddBoolOption((XamlStylerSettings x) => x.FormatMarkupExtension, "Enable markup extension formatting");
            var noNewLineMarkupExtensionsOption = AddTextBox((XamlStylerSettings x) => x.NoNewLineMarkupExtensions, "Keep markup extensions of these types on one line:");
            
            AddBinding(noNewLineMarkupExtensionsOption, BindingStyle.IsEnabledProperty,
                (XamlStylerSettings x) => x.FormatMarkupExtension,
                x => (bool)x);
            
            // Thickness formatting
            AddHeader("Thickness formatting");
            AddComboEnum((XamlStylerSettings x) => x.ThicknessStyle, "Thickness separator:");
            AddTextBox((XamlStylerSettings x) => x.ThicknessAttributes, "Thickness attributes:");

            // Misc
            AddHeader("Misc");
            AddSpinner((XamlStylerSettings x) => x.CommentSpaces, "Comment padding:");
            
            // Configuration
            AddHeader("Configuration");
            AddText("External configuration file:");
            var configPath = new Property<FileSystemPath>(lifetime, "XamlStylerOptionsPage::configPath");
            OptionsSettingsSmartContext.SetBinding(lifetime, (XamlStylerSettings k) => k.ConfigPath, configPath);
            AddFileChooserOption(configPath, "External configuration file", FileSystemPath.Empty, null, commonFileDialogs, null, false, "", null, null, null, null);
            AddBoolOption((XamlStylerSettings x) => x.SearchToDriveRoot, "Search to drive root");
            AddBoolOption((XamlStylerSettings x) => x.SuppressProcessing, "Suppress processing");
        }

        private BeTextBox AddTextBox<TKeyClass>(Expression<Func<TKeyClass, string>> lambdaExpression,
            string description)
        {
            var property = new Property<string>(description);
            OptionsSettingsSmartContext.SetBinding(Lifetime, lambdaExpression, property);
            var control = property.GetBeTextBox(Lifetime);
            AddKeyword(description);
            AddControl(control
                .WithMaxSize(BeControlSizes.GetSize(BeControlSizeType.LARGE, BeControlSizeType.FIT_TO_CONTENT))
                .WithDescription(description, Lifetime, size: BeSizingType.Fill));
            
            return control;
        }
        
        private BeSpinner AddSpinner<TKeyClass>(Expression<Func<TKeyClass, int>> lambdaExpression,
            string description, int min = int.MinValue, int max = int.MaxValue)
        {
            var property = new Property<int>(description);
            OptionsSettingsSmartContext.SetBinding(Lifetime, lambdaExpression, property);
            var control = property.GetBeSpinner(Lifetime, min, max);
            AddKeyword(description);
            AddControl(control
                .WithDescription(description, Lifetime, size: BeSizingType.Fit));
            
            return control;
        }
        
        private BeControl AddListControl<TKeyClass>(Expression<Func<TKeyClass, string>> lambdaExpression,
            string[] defaults, string description)
        {
            var property = new Property<string>(description);
            OptionsSettingsSmartContext.SetBinding(Lifetime, lambdaExpression, property);
            if (string.IsNullOrEmpty(property.Value))
            {
                property.Value = string.Join("\n", defaults);
            }
            
            var model = new StringListViewModel(Lifetime, property);

            var list = model.SelectedEntry.GetBeSingleSelectionListWithToolbar(
                    model.Entries,
                    Lifetime,
                    (entryLt, entry, properties) => new List<BeControl>
                    {
                        entry.Value.GetBeTextBox(entryLt)
                            .WithValidationRule<BeTextBox, string>(
                                Lifetime,
                                value => !value.IsEmpty(),
                                "Value must not be empty.")
                    },
                    iconHost: _iconHost,
                    columnsAndSizes: new[] {"Value,*"},
                    dock: BeDock.RIGHT)
                .AddButtonWithListAction(BeListAddAction.ADD, i => model.CreateNewEntry())
                .AddButtonWithListAction<StringListViewModel.StringListEntry>(BeListAction.REMOVE, i => model.OnEntryRemoved())
                .AddButtonWithListAction<StringListViewModel.StringListEntry>(BeListAction.MOVE_UP, i => model.OnEntryMoved())
                .AddButtonWithListAction<StringListViewModel.StringListEntry>(BeListAction.MOVE_DOWN, i => model.OnEntryMoved());

            var grid = BeControls.GetGrid();
            var label = description.GetBeLabel();
            grid.AddElement(label);
            grid.AddElement(list);
            AddKeyword(description);
            
            AddControl(grid.WithMargin(BeMargins.Create(SideMargin, TopMargin, SideMargin)));

            return list;
        }
    }
}