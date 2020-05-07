using System;
using System.IO;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Format;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Xaml;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using Xavalon.XamlStyler.Core.Options;

namespace ReSharperPlugin.XamlStyler.dotUltimate
{
    public static class StylerOptionsFactory
    {
        public static IStylerOptions FromSettings(
            IContextBoundSettingsStoreLive settings,
            [CanBeNull] ISolution solution,
            [CanBeNull] IProject project,
            [CanBeNull] IPsiSourceFileWithLocation psiSourceFileWithLocation)
        {
            return FromSettings(
                settings: settings,
                solution: solution, 
                projectPath: project?.ProjectFileLocation?.FullPath, 
                sourceFilePath: psiSourceFileWithLocation?.Location.FullPath);
        }
        
         public static IStylerOptions FromSettings(
            IContextBoundSettingsStoreLive settings,
            [CanBeNull] ISolution solution,
            [CanBeNull] string projectPath,
            [CanBeNull] string sourceFilePath)
        {
            // 1. Load global settings
            IStylerOptions fallbackOptions = new StylerOptions();
            IStylerOptions stylerOptions = new StylerOptions();
            
            stylerOptions.IndentSize = settings.GetValue((XamlStylerSettings s) => s.IndentSize);
            stylerOptions.IndentWithTabs = settings.GetValue((XamlStylerSettings s) => s.IndentWithTabs);
            
            stylerOptions.AttributesTolerance = settings.GetValue((XamlStylerSettings s) => s.AttributesTolerance);
            stylerOptions.KeepFirstAttributeOnSameLine = settings.GetValue((XamlStylerSettings s) => s.KeepFirstAttributeOnSameLine);
            stylerOptions.MaxAttributeCharactersPerLine = settings.GetValue((XamlStylerSettings s) => s.MaxAttributeCharactersPerLine);
            stylerOptions.MaxAttributesPerLine = settings.GetValue((XamlStylerSettings s) => s.MaxAttributesPerLine);
            stylerOptions.NoNewLineElements = settings.GetValue((XamlStylerSettings s) => s.NoNewLineElements);
            stylerOptions.PutAttributeOrderRuleGroupsOnSeparateLines = settings.GetValue((XamlStylerSettings s) => s.PutAttributeOrderRuleGroupsOnSeparateLines);
            stylerOptions.AttributeIndentation = settings.GetValue((XamlStylerSettings s) => s.AttributeIndentation);
            stylerOptions.AttributeIndentationStyle = settings.GetValue((XamlStylerSettings s) => s.AttributeIndentationStyle);
            stylerOptions.RemoveDesignTimeReferences = settings.GetValue((XamlStylerSettings s) => s.RemoveDesignTimeReferences);
            stylerOptions.EnableAttributeReordering = settings.GetValue((XamlStylerSettings s) => s.EnableAttributeReordering);
            stylerOptions.AttributeOrderingRuleGroups = settings.GetValue((XamlStylerSettings s) => s.AttributeOrderingRuleGroups)?.SplitByNewLine();
            if (stylerOptions.AttributeOrderingRuleGroups == null || stylerOptions.AttributeOrderingRuleGroups.Length == 0)
            {
                stylerOptions.AttributeOrderingRuleGroups = fallbackOptions.AttributeOrderingRuleGroups;
            }
            stylerOptions.FirstLineAttributes = settings.GetValue((XamlStylerSettings s) => s.FirstLineAttributes);
            stylerOptions.OrderAttributesByName = settings.GetValue((XamlStylerSettings s) => s.OrderAttributesByName);
            stylerOptions.PutEndingBracketOnNewLine = settings.GetValue((XamlStylerSettings s) => s.PutEndingBracketOnNewLine);
            stylerOptions.RemoveEndingTagOfEmptyElement = settings.GetValue((XamlStylerSettings s) => s.RemoveEndingTagOfEmptyElement);
            stylerOptions.SpaceBeforeClosingSlash = settings.GetValue((XamlStylerSettings s) => s.SpaceBeforeClosingSlash);
            stylerOptions.RootElementLineBreakRule = settings.GetValue((XamlStylerSettings s) => s.RootElementLineBreakRule);
            stylerOptions.ReorderVSM = settings.GetValue((XamlStylerSettings s) => s.ReorderVSM);
            stylerOptions.ReorderGridChildren = settings.GetValue((XamlStylerSettings s) => s.ReorderGridChildren);
            stylerOptions.ReorderCanvasChildren = settings.GetValue((XamlStylerSettings s) => s.ReorderCanvasChildren);
            stylerOptions.ReorderSetters = settings.GetValue((XamlStylerSettings s) => s.ReorderSetters);
            stylerOptions.FormatMarkupExtension = settings.GetValue((XamlStylerSettings s) => s.FormatMarkupExtension);
            stylerOptions.NoNewLineMarkupExtensions = settings.GetValue((XamlStylerSettings s) => s.NoNewLineMarkupExtensions);
            stylerOptions.ThicknessStyle = settings.GetValue((XamlStylerSettings s) => s.ThicknessStyle);
            stylerOptions.ThicknessAttributes = settings.GetValue((XamlStylerSettings s) => s.ThicknessAttributes);
            stylerOptions.FormatOnSave = settings.GetValue((XamlStylerSettings s) => s.FormatOnSave);
            stylerOptions.CommentSpaces = settings.GetValue((XamlStylerSettings s) => s.CommentSpaces);
            stylerOptions.ConfigPath = settings.GetValue((XamlStylerSettings s) => s.ConfigPath)?.FullPath;
            stylerOptions.SearchToDriveRoot = settings.GetValue((XamlStylerSettings s) => s.SearchToDriveRoot);
            stylerOptions.SuppressProcessing = settings.GetValue((XamlStylerSettings s) => s.SuppressProcessing);
            
            // 2. Try finding settings in our project/solution?
            if (!string.IsNullOrEmpty(projectPath) || !string.IsNullOrEmpty(sourceFilePath))
            {
                var searchToDriveRoot = settings.GetValue((XamlStylerSettings s) => s.SearchToDriveRoot);
                
                var highestRootPath = solution != null && !solution.IsTemporary
                    ? (searchToDriveRoot ? Path.GetPathRoot(solution.SolutionFilePath.FullPath) : Path.GetDirectoryName(solution.SolutionFilePath.FullPath))
                    : string.Empty;

                var itemPath = sourceFilePath;
                
                var configPath = (!string.IsNullOrEmpty(itemPath) && itemPath.StartsWith(highestRootPath, StringComparison.OrdinalIgnoreCase))
                    ? GetConfigPathForProject(highestRootPath, itemPath)
                    : GetConfigPathForProject(projectPath ?? itemPath, itemPath);
                if (!string.IsNullOrEmpty(configPath))
                {
                    stylerOptions = ((StylerOptions)stylerOptions).Clone();
                    stylerOptions.ConfigPath = configPath;
                }
            }
            
            // 3. Override with IDE-specifics
            var xamlFormatter = XamlLanguage.Instance.Formatter<ICodeFormatterImpl>();
            if (xamlFormatter != null)
            {
                // Note: stylerOptions.UseVisualStudioIndentSize is hardcoded to "True", which means we'll always use IDE settings when in IDE context.
                // To overcome this, we're ignoring the setting from XamlStyler settings files, and using the configuration in the IDE, so we can toggle this on/off.
                var schema = Shell.Instance.GetComponent<ISettingsSchema>();
                if (/*stylerOptions.UseVisualStudioIndentSize ||*/ settings.GetValue((XamlStylerSettings s) => s.UseIdeIndentSize))
                {
                    stylerOptions.IndentSize = (int)xamlFormatter.GetEntry(schema, key => key.INDENT_SIZE).GetDefaultValueInEntryMemberType();
                }
                
                if (/*stylerOptions.UseVisualStudioIndentWithTabs ||*/ settings.GetValue((XamlStylerSettings s) => s.UseIdeIndentWithTabs))
                {
                    var ideIndentStyle = (IndentStyle)xamlFormatter.GetEntry(schema, key => key.INDENT_SIZE).GetDefaultValueInEntryMemberType();
                    stylerOptions.IndentWithTabs = ideIndentStyle == IndentStyle.Tab;
                }
            }

            return stylerOptions;
        }
        
        private static string GetConfigPathForProject(string highestRootPath, string path)
        {
            if (path.IsNullOrEmpty())
            {
                return null;
            }
            
            var currentDirectory = Path.GetDirectoryName(path);
            while (currentDirectory?.StartsWith(highestRootPath, StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                var configurationFilePath = Path.Combine(currentDirectory, "Settings.XamlStyler");
                if (File.Exists(configurationFilePath))
                {
                    return configurationFilePath;
                }
                
                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            return null;
        }
    }
}