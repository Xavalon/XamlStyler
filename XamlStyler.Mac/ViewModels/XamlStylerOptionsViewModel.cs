using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xavalon.XamlStyler.Core.Options;
using Xavalon.XamlStyler.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Mac.ViewModels
{
    public class XamlStylerOptionsViewModel
    {
        private IXamlStylerOptionsService XamlStylerOptionsService => Container.Instance.Resolve<IXamlStylerOptionsService>();

        public IList<IGrouping<string, XamlStylerOptionViewModel>> GroupedOptions { get; private set; }

        public IStylerOptions Options { get; private set; }

        public bool IsDirty { get; set; }

        public void RefreshData()
        {
            Options = XamlStylerOptionsService.GetGlobalOptions();

            var properties = TypeDescriptor.GetProperties(Options);

            GroupedOptions = properties.Cast<PropertyDescriptor>()
                                       .Select(property => new XamlStylerOptionViewModel(property))
                                       .Where(option => option.IsConfigurable)
                                       .GroupBy(option => option.Category)
                                       .ToList();
        }

        public void SaveOptions()
        {
            if (!IsDirty)
            {
                return;
            }

            XamlStylerOptionsService.SaveGlobalOptions(Options);
        }

        public void ResetToDefaults()
        {
            XamlStylerOptionsService.ResetGlobalOptions();
            IsDirty = false;
        }
    }
}