using System;
using Microsoft.VisualStudio.Text.Editor;
using System.IO;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Xavalon.XamlStyler.Core;
using Microsoft.VisualStudio.Text;
using Xavalon.XamlStyler.Core.Options;
using MonoDevelop.Core;

namespace Xavalon.XamlStyler.Mac.Services.XamlStyler
{
    public class XamlStylerService : IXamlStylerService
    {
        private const string XamlFileExtension = ".xaml";

        public void FormatAndSaveXamlFile(string filePath)
        {
            var options = StylerOptionsConfiguration.GetOptionsForDocument(file.Name, file.Project);
            var styler = new Core.StylerService(options);

            LoggingService.LogDebug($"Processing {file.FilePath} in-place");
            var content = System.IO.File.ReadAllText(file.FilePath);

            var styledXaml = styler.StyleDocument(content);

            System.IO.File.WriteAllText(file.FilePath, styledXaml);

            var openedFile = IdeApp.Workbench.Documents.FirstOrDefault(f => f.FileName.FullPath == file.FilePath);
            if (openedFile != null)
            {
                LoggingService.LogDebug($"Reloading {file.FilePath} in editor window");
                openedFile.Reload();
            }
        }

        public void FormatXamlFile(Document fileDocument)
        {
            var filePath = fileDocument?.FilePath;
            if (!IsFormatableFile(filePath))
            {
                return;
            }

            var stylerOptions = StylerOptionsConfiguration.GetOptionsForDocument(fileDocument.FileName, fileDocument.Owner as Project);
            var stylerService = new Core.StylerService(stylerOptions);

            var fileTextBuffer = fileDocument.TextBuffer;
            var currentTextSnapshot = fileTextBuffer.CurrentSnapshot;
            var currentText = currentTextSnapshot.GetText();
            var styledText = stylerService.StyleDocument(currentText);
            if (currentText == styledText)
            {
                return;
            }

            var replaceSpan = new Span(0, currentText.Length);
            fileTextBuffer.Replace(replaceSpan, styledText);

            fileDocument.IsDirty = true;
        }

        public bool IsFormatableFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var extension = Path.GetExtension(filePath);
            var isXamlFile = string.Equals(extension, XamlFileExtension, StringComparison.InvariantCultureIgnoreCase);
            return isXamlFile;
        }

        public StylerOptions GetOptionsForFile(string filePath, Project project)
        {
            var stylerOptions = ReadFromUserProfile();
            var configPath = GetConfigPathForDocument(filePath, project);

            if (configPath != null)
            {
                stylerOptions = (XamlStylerOptions)stylerOptions.Clone();
                stylerOptions.ConfigPath = configPath;
            }

            //TODO Add this feature support
            if (stylerOptions.UseVisualStudioIndentSize)
            {
                stylerOptions.IndentSize = 4;
            }

            return stylerOptions;
        }


        public StylerOptions ReadFromUserProfile()
        {
            var filePath = GetOptionsFilePath().ToString();
            try
            {
                var optionsJsonString = File.ReadAllText(filePath);
                var stylerOptions = JsonConvert.DeserializeObject<XamlStylerOptions>(optionsJsonString);
                return stylerOptions;
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Exception when saving user XamlStyler options", ex);
                File.Delete(filePath);
            }

            // Xamarin Forms defaults
            var options = new XamlStylerOptions()
            {
                IndentSize = 4
            };

            try
            {
                // update attribute ordering to include Forms attrs
                options.AttributeOrderingRuleGroups[6] += ", WidthRequest, HeightRequest";
                options.AttributeOrderingRuleGroups[7] += ", HorizontalOptions, VerticalOptions, XAlign, VAlign";
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Exception when updating default options to include Xamarin Forms attributes", ex);
            }

            return options;
        }

        private static string GetConfigPathForDocument(string documentPath, Project project)
        {
            try
            {
                if (string.IsNullOrEmpty(documentPath) || project is null)
                {
                    return null;
                }

                var projectFileName = project.FileName;
                var projectRoot = string.IsNullOrEmpty(projectFileName) ? string.Empty : Path.GetDirectoryName(projectFileName);

                var solution = project.ParentSolution;
                var solutionFileName = solution.FileName;
                var solutionRoot = string.IsNullOrEmpty(solutionFileName) ? string.Empty : Path.GetDirectoryName(solutionFileName);

                var root = documentPath.StartsWith(solutionRoot, StringComparison.OrdinalIgnoreCase) ? solutionRoot : projectRoot;
                var configPaths = GetConfigPathBetweenPaths(documentPath, root);

                return configPaths.FirstOrDefault(File.Exists);
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Failed to find config path", ex);
                return null;
            }
        }
    }
}
