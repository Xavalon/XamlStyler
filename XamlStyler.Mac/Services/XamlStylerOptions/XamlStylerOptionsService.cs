using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Newtonsoft.Json;
using System;
using System.IO;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Mac.Services.XamlStylerOptions
{
    public class XamlStylerOptionsService : IXamlStylerOptionsService
    {
        private const string OptionsFileName = "Settings.XamlStyler";

        private string GlobalOptionsFilePath => UserProfile.Current.ConfigDir.Combine(OptionsFileName).FullPath;

        public IStylerOptions GetGlobalOptions()
        {
            var defaultGlobalOptions = new StylerOptions
            {

                // TODO Check info about IndentSize, is it necessary
                IndentSize = 4
            };

            var globalOptions = ParseOptionsOrDefault(GlobalOptionsFilePath, defaultGlobalOptions);
            return globalOptions;
        }

        public void SaveGlobalOptions(IStylerOptions options)
        {
            try
            {
                var fileData = JsonConvert.SerializeObject(options);
                File.WriteAllText(GlobalOptionsFilePath, fileData);
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Failed to save Global XamlStyler options", ex);
            }
        }

        public IStylerOptions GetDocumentOptions(Document document)
        {
            var globalOptions = GetGlobalOptions();
            var project = document?.Owner as Project;
            var solution = project?.ParentSolution;
            if (solution is null)
            {
                return globalOptions;
            }

            var optionsRootFolder = solution.RootFolder.Name;
            if (globalOptions.SearchToDriveRoot)
            {
                optionsRootFolder = solution.RootFolder.ParentFolder.Name;
            }

            var firstOptionsFilePath = GetFirstOptionsFilePathOrDefault(document.FileName, optionsRootFolder);
            if (!string.IsNullOrEmpty(firstOptionsFilePath))
            {
                var firstOptions = ParseOptionsOrDefault(firstOptionsFilePath, globalOptions);
                return firstOptions;
            }

            var externalOptionsFilePath = globalOptions.ConfigPath;
            if (!string.IsNullOrEmpty(externalOptionsFilePath))
            {
                var externalOptions = ParseOptionsOrDefault(externalOptionsFilePath, globalOptions);
                return externalOptions;
            }

            return globalOptions;
        }

        private IStylerOptions ParseOptionsOrDefault(string optionsFilePath, IStylerOptions defaultOptions = null)
        {
            try
            {
                if (string.IsNullOrEmpty(optionsFilePath) || !File.Exists(optionsFilePath))
                {
                    return defaultOptions;
                }

                var optionsString = File.ReadAllText(optionsFilePath);
                var options = JsonConvert.DeserializeObject<StylerOptions>(optionsString);
                return options;
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Failed to get Global XamlStyler options", ex);
                File.Delete(GlobalOptionsFilePath);
                return defaultOptions;
            }
        }

        private string GetFirstOptionsFilePathOrDefault(string documentFilePath, string rootPath)
        {
            var currentDirectory = Path.GetDirectoryName(documentFilePath);
            var currentConfigPath = Path.Combine(currentDirectory, OptionsFileName);
            while (!File.Exists(currentConfigPath) && currentConfigPath.StartsWith(rootPath, StringComparison.InvariantCultureIgnoreCase))
            {
                currentDirectory = Path.GetDirectoryName(currentDirectory);
                currentConfigPath = Path.Combine(currentDirectory, OptionsFileName);
            }

            if (!File.Exists(currentConfigPath))
            {
                return null;
            }

            return currentConfigPath;
        }
    }
}