using System;
using MonoDevelop.Ide.Gui;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Extension.Mac.Extensions
{
    public static class DocumentExtensions
    {
        public static bool IsXaml(this Document document)
        {
            var fileExtension = document.FilePath.Extension;
            var isXamlFile = string.Equals(fileExtension, Constants.XamlFileExtension, StringComparison.InvariantCultureIgnoreCase);
            return isXamlFile;
        }

        public static bool IsXamarinXaml(this Document document)
        {
            var fileExtension = document.FilePath.Extension;
            var isXamlFile = string.Equals(fileExtension, Constants.XamlFileExtension, StringComparison.InvariantCultureIgnoreCase);
            return isXamlFile;
        }

        public static bool IsAvaloniaXaml(this Document document)
        {
            var fileExtension = document.FilePath.Extension;
            var isXamlFile = string.Equals(fileExtension, Constants.AxamlFileExtension, StringComparison.InvariantCultureIgnoreCase);
            return isXamlFile;
        }

        public static XamlLanguageOptions GetXamlLanguageOptions(this Document document)
        {
            var options = new XamlLanguageOptions();

            if (document == null || document.IsViewOnly)
            {
                return options;
            }

            if (document.IsAvaloniaXaml())
            {
                options.IsFormatable = true;
                options.UnescapedAttributeCharacters.Add('>');

                return options;
            }

            options.IsFormatable = document.IsXaml() || document.IsXamarinXaml();

            return options;
        }
    }
}

