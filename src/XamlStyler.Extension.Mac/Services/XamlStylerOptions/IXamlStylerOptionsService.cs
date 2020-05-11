// © Xavalon. All rights reserved.

using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Extension.Mac.Services.XamlStylerOptions
{
    public interface IXamlStylerOptionsService
    {
        IStylerOptions GetGlobalOptions();

        void SaveGlobalOptions(IStylerOptions options);

        void ResetGlobalOptions();

        IStylerOptions GetDocumentOptions(Document document);

        IStylerOptions GetDocumentOptions(string documentFilePath, Solution solution);
    }
}