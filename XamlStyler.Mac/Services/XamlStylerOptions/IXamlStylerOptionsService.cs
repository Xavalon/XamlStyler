using MonoDevelop.Ide.Gui;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Mac.Services.XamlStylerOptions
{
    public interface IXamlStylerOptionsService
    {
        IStylerOptions GetGlobalOptions();

        void SaveGlobalOptions(IStylerOptions options)

        IStylerOptions GetDocumentOptions(Document document);
    }
}
