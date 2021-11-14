// © Xavalon. All rights reserved.

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;

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

        public static bool IsFormatable(this Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return (document != null)
                ? !document.ReadOnly && (document.IsXaml() || document.IsXamarinXaml())
                : false;
        }
    }
}
