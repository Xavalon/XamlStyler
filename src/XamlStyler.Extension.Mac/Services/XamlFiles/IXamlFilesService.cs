// (c) Xavalon. All rights reserved.

using MonoDevelop.Projects;
using System.Collections.Generic;

namespace Xavalon.XamlStyler.Extension.Mac.Services.XamlFiles
{
    public interface IXamlFilesService
    {
        List<string> FindAllXamlFilePaths(Solution solution);

        List<string> FindAllXamlFilePaths(Project project);
    }
}