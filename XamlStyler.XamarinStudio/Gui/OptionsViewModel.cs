using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.XamarinStudio.Gui
{
	public class OptionsViewModel
	{
		public IList<IGrouping<string, Option>> GroupedOptions { get; private set; }

		public StylerOptions Options { get; private set; }

		public void Init()
		{
			Options = ReadOptions();
			
			var optionsList = new List<Option>();
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(Options))
			{
				optionsList.Add(new Option(property));
			}

			GroupedOptions = optionsList.Where(o => o.IsConfigurable).GroupBy(o => o.Category).ToList();
		}

		public StylerOptions ReadOptions()
		{
			return StylerOptionsConfiguration.ReadFromUserProfile();
		}

		public void SaveOptions()
		{
			StylerOptionsConfiguration.WriteToUserProfile(Options);
		}

		public void ResetToDefaults()
		{
			StylerOptionsConfiguration.Reset();
		}
	}
}

