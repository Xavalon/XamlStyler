using System;
using System.Collections.Generic;
using System.IO;
using MonoDevelop.Projects;
using System.Linq;

namespace Xavalon.XamlStyler.Mac.Services.XamlFiles
{
    public class XamlFilesService : IXamlFilesService
    {
        public List<string> FindAllXamlFilePaths(Solution solution)
        {
            return solution.GetAllProjects()
                           .SelectMany(FindAllXamlFilePaths)
                           .ToList();
        }

        public List<string> FindAllXamlFilePaths(Project project)
        {
            return project.Files
                          .Where(IsXamlFile)
                          .Select(file => file.FilePath.FileName)
                          .ToList();
        }

        private bool IsXamlFile(ProjectFile file)
        {
            var fileExtension = file.FilePath.Extension;
            var isXamlFile = string.Equals(fileExtension, Constants.XamlFileExtension, StringComparison.InvariantCultureIgnoreCase);
            return isXamlFile;
        }
    }
}
