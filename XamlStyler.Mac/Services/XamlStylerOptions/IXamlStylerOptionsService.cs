using MonoDevelop.Ide.Gui;
using Xavalon.XamlStyler.Core.Options;
using MonoDevelop.Projects;

namespace Xavalon.XamlStyler.Mac.Services.XamlStylerOptions
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