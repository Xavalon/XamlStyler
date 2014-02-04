using Microsoft.VisualStudio.Shell;
using XamlStyler.Core.Options;

namespace XamlStyler.VSPackage
{
    public class PackageOptions : DialogPage
    {
        private readonly IStylerOptions _options;

        #region Constructors

        public PackageOptions()
        {
            _options = new StylerOptions();
        }

        #endregion Constructors

        public override object AutomationObject
        {
            get { return _options; }
        }
    }
}