using MonoDevelop.Ide.Gui;

namespace Xavalon.XamlStyler.Mac.Services.XamlStyler
{
    public interface IXamlStylerService
    {
        void FormatAndSaveXamlFile(string filePath);

        void FormatXamlFile(Document fileDocument);

        bool IsFormatableFile(string filePath);
    }
}