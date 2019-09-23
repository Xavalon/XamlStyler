using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Mac
{
    public static class StylerOptionsConfiguration
    {
        public static bool IsFormatableDocument(Document document)
        {
            if (document is null || document.IsViewOnly)
            {
                return false;
            }

            var isFormatableDocument = document.FileName.FileName.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase);
            return isFormatableDocument;
        }

        public static StylerOptions GetOptionsForDocument(string documentPath, Project project)
        {
            var stylerOptions = ReadFromUserProfile();
            var configPath = GetConfigPathForDocument(documentPath, project);

            if (configPath != null)
            {
                stylerOptions = (StylerOptions)stylerOptions.Clone();
                stylerOptions.ConfigPath = configPath;
            }

            //TODO Add this feature support
            if (stylerOptions.UseVisualStudioIndentSize)
            {
                stylerOptions.IndentSize = 4;
            }

            return stylerOptions;
        }

        public static StylerOptions ReadFromUserProfile()
        {
            var filePath = GetOptionsFilePath().ToString();
            try
            {
                var optionsJsonString = File.ReadAllText(filePath);
                var stylerOptions = JsonConvert.DeserializeObject<StylerOptions>(optionsJsonString);
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
            var options = new StylerOptions()
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

        public static void WriteToUserProfile(StylerOptions options)
        {
            try
            {
                var text = JsonConvert.SerializeObject(options);
                File.WriteAllText(GetOptionsFilePath().ToString(), text);
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Exception when saving user XamlStyler options", ex);
            }
        }

        public static void Reset()
        {
            File.Delete(GetOptionsFilePath().ToString());
        }

        private static FilePath GetOptionsFilePath()
        {
            return UserProfile.Current.ConfigDir.Combine("xamlstyler.config");
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

        /// <summary>
        /// Searches for configuration file up through solution root directory
        /// </summary>
        private static IEnumerable<string> GetConfigPathBetweenPaths(string path, string root)
        {
            var configDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory) ? path : Path.GetDirectoryName(path);

            while (configDirectory.StartsWith(root, StringComparison.InvariantCultureIgnoreCase))
            {
                yield return Path.Combine(configDirectory, "Settings.XamlStyler");
                configDirectory = Path.GetDirectoryName(configDirectory);
            }
        }
    }
}

