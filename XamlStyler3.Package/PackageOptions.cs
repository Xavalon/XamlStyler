using Microsoft.VisualStudio.Shell;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Package
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