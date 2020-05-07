// © Xavalon. All rights reserved.

using Microsoft.VisualStudio.Shell;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Package
{
    public class PackageOptions : DialogPage
    {
        private readonly IStylerOptions options;

        public PackageOptions()
        {
            this.options = new StylerOptions();
        }

        public override object AutomationObject => this.options;
    }
}