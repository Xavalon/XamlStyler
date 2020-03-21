// © Xavalon. All rights reserved.

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;

namespace Xavalon.XamlStyler.Package.Extensions
{
    public static class ProjectItemExtensions
    {
        public static bool IsXaml(this ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return projectItem.GetFileName().EndsWith(Constants.XamlFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsFormatable(this ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return (projectItem != null) && (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile);
        }

        public static string GetFileName(this ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                return projectItem.FileNames[1];
            }
            catch
            {
                return null;
            }
        }
    }
}
