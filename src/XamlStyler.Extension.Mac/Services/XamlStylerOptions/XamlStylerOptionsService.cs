// (c) Xavalon. All rights reserved.

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Newtonsoft.Json;
using System;
using System.IO;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Extension.Mac.Converters;
using Xavalon.XamlStyler.Extension.Mac.Utils;

namespace Xavalon.XamlStyler.Extension.Mac.Services.XamlStylerOptions
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

            var globalOptions = ParseOptionsOrDefault(GlobalOptionsFilePath, defaultGlobalOptions, new GlobalOptionsJsonConverter());
            return globalOptions;
        }

        public void SaveGlobalOptions(IStylerOptions options)
        {
            try
            {
                var fileData = JsonConvert.SerializeObject(options, new GlobalOptionsJsonConverter());
                File.WriteAllText(GlobalOptionsFilePath, fileData);
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Failed to save Global XamlStyler options", ex);
            }
        }

        public void ResetGlobalOptions()
        {
            File.Delete(GlobalOptionsFilePath);
        }

        public IStylerOptions GetDocumentOptions(Document document)
        {
            var documentFilePath = document?.FilePath.ToString();
            var project = document?.Owner as Project;
            var solution = project?.ParentSolution;

            return GetDocumentOptions(documentFilePath, solution);
        }

        public IStylerOptions GetDocumentOptions(string documentFilePath, Solution solution)
        {
            var globalOptions = GetGlobalOptions();
            if (string.IsNullOrEmpty(documentFilePath) || solution is null)
            {
                return globalOptions;
            }

            var optionsRootFolder = solution.FileName.ParentDirectory;
            if (globalOptions.SearchToDriveRoot)
            {
                optionsRootFolder = optionsRootFolder.ParentDirectory;
            }

            var optionsRootFolderPath = optionsRootFolder.ToString();

            var firstOptionsFilePath = GetFirstOptionsFilePathOrDefault(documentFilePath, optionsRootFolderPath);
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

        private IStylerOptions ParseOptionsOrDefault(string optionsFilePath, IStylerOptions defaultOptions, JsonConverter deserializeConverter = null)
        {
            try
            {
                optionsFilePath = PathUtils.ToAbsolutePath(optionsFilePath);
                if (string.IsNullOrEmpty(optionsFilePath) || !File.Exists(optionsFilePath))
                {
                    return defaultOptions;
                }

                var optionsString = File.ReadAllText(optionsFilePath);
                var converters = deserializeConverter is null ? new JsonConverter[0] : new[] { deserializeConverter };
                var options = JsonConvert.DeserializeObject<StylerOptions>(optionsString, converters);
                if (options.IndentSize == -1)
                {
                    // TODO Check info about IndentSize, is it necessary
                    options.IndentSize = 4;
                }

                // TODO Remove it when we will handle Indent from Visual Studio preferences
                options.IndentWithTabs |= defaultOptions.IndentWithTabs;

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
            var currentParentDirectory = Path.GetDirectoryName(currentDirectory);
            var currentConfigPath = Path.Combine(currentDirectory, OptionsFileName);
            while (!File.Exists(currentConfigPath) && currentParentDirectory.StartsWith(rootPath, StringComparison.InvariantCultureIgnoreCase))
            {
                currentDirectory = Path.GetDirectoryName(currentDirectory);
                currentParentDirectory = Path.GetDirectoryName(currentDirectory);
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