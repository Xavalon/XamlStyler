// © Xavalon. All rights reserved.

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Extension.Windows.Extensions
{
    public static class DocumentExtensions
    {
        public static bool IsXaml(this Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return String.Equals(document.Language, Constants.XamlLanguageType, StringComparison.OrdinalIgnoreCase)
                || document.FullName.EndsWith(Constants.XamlFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsXamarinXaml(this Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return String.Equals(document.Language, Constants.XmlLanguageType, StringComparison.OrdinalIgnoreCase)
                && document.FullName.EndsWith(Constants.XamlFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsAvaloniaXaml(this Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return String.Equals(document.Language, Constants.XmlLanguageType, StringComparison.OrdinalIgnoreCase)
                   && document.FullName.EndsWith(Constants.AxamlFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public static XamlLanguageOptions GetXamlLanguageOptions(this Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var options = new XamlLanguageOptions();

            if (document == null || document.ReadOnly)
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
