using Microsoft.VisualStudio.Shell;
using XamlStyler.Core.Options;

namespace NicoVermeir.XamlStyler_Package
{
    public class PackageOptions : DialogPage
    {
        private readonly IStylerOptions _options;

        public PackageOptions()
        {
            _options = new StylerOptions();
        }

        public override object AutomationObject
        {
            get { return _options; }
        }
    }
}