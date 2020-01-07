using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Mac.Gui
{
	public class OptionsViewModel
	{
        private readonly string[] _disabledCategories = { "XAML Styler Configuration" };

		public IList<IGrouping<string, OptionViewModel>> GroupedOptions { get; private set; }

		public StylerOptions Options { get; private set; }

		public bool IsDirty { get; set; }

		public void Initialize()
		{
			Options = ReadOptions();

            var properties = TypeDescriptor.GetProperties(Options);

            GroupedOptions = properties.Cast<PropertyDescriptor>()
                                       .Select(property => new OptionViewModel(property))
                                       .Where(option => option.IsConfigurable && !_disabledCategories.Contains(option.Category))
                                       .GroupBy(option => option.Category)
                                       .ToList();
		}

		public StylerOptions ReadOptions()
		{
			return StylerOptionsConfiguration.ReadFromUserProfile();
		}

		public void SaveOptions()
		{
			if (IsDirty)
			{
				StylerOptionsConfiguration.WriteToUserProfile(Options);
			}
		}

		public void ResetToDefaults()
		{
			StylerOptionsConfiguration.Reset();
			IsDirty = false;
		}
	}
}