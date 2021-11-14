// © Xavalon. All rights reserved.

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xavalon.XamlStyler.Extension.Windows.Helpers
{
    public static class ProjectItemHelper
    {
        public static IEnumerable<ProjectItem> GetAllProjectItems(Solution solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var allProjects = new List<ProjectItem>();

            if (solution != null)
            {
                IEnumerable<ProjectItem> projectItems = GetProjectItemsRecursively(solution);
                allProjects.AddRange(projectItems);
            }

            return allProjects;
        }

        public static IEnumerable<ProjectItem> GetProjectItemsRecursively(object parentItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (parentItem == null)
            {
                throw new ArgumentNullException(nameof(parentItem));
            }

            var projectItems = new List<ProjectItem>();

            if (parentItem is ProjectItem projectItem)
            {
                projectItems.Add(projectItem);
            }

            foreach (object childItem in GetChildren(parentItem))
            {
                projectItems.AddRange(GetProjectItemsRecursively(childItem));
            }

            return projectItems;
        }

        public static IEnumerable<ProjectItem> GetSelectedProjectItemsRecursively(StylerPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var selectedProjectItems = new List<ProjectItem>();

            UIHierarchy solutionExplorer = package.IDE2.ToolWindows.SolutionExplorer;
            IEnumerable<UIHierarchyItem> selectedUIHierarchyItems =
                ((object[])solutionExplorer.SelectedItems).Cast<UIHierarchyItem>().ToList();

            IEnumerable<object> selectedItems = selectedUIHierarchyItems
                .Select(uiHierarchyItem => {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return uiHierarchyItem.Object;
                });

            foreach (object item in selectedItems)
            {
                selectedProjectItems.AddRange(GetProjectItemsRecursively(item));
            }

            return selectedProjectItems;
        }

        private static IEnumerable<object> GetChildren(object parentItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Check if item is a solution.
            var solution = parentItem as Solution;
            if (solution?.Projects != null)
            {
                return solution.Projects.Cast<Project>().Cast<object>().ToList();
            }

            // Check if item is a project.
            var project = parentItem as Project;
            if (project?.ProjectItems != null)
            {
                return project.ProjectItems.Cast<ProjectItem>().Cast<object>().ToList();
            }

            // Check if item is a project item.
            if (parentItem is ProjectItem projectItem)
            {
                // Standard projects.
                if (projectItem.ProjectItems != null)
                {
                    return projectItem.ProjectItems.Cast<ProjectItem>().Cast<object>().ToList();
                }

                // Projects within a solution folder.
                if (projectItem.SubProject != null)
                {
                    return new[] { projectItem.SubProject };
                }
            }

            return Array.Empty<object>();
        }
    }
}