using System;
using System.ComponentModel;

namespace Xavalon.XamlStyler.VisualStudioForMac.Gui
{
	public class Option
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Category { get; set; }

		public bool IsConfigurable { get; set; } = true;

		public Type PropertyType { get; set; }

		public PropertyDescriptor Property { get; set; }

		public Option(PropertyDescriptor property)
		{
			var nameAttr = property.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
			var descAttr = property.Attributes[typeof(DescriptionAttribute)] as DescriptionAttribute;
			var categoryAttr = property.Attributes[typeof(CategoryAttribute)] as CategoryAttribute;

			if (nameAttr != null)
			{
				if (!string.IsNullOrEmpty(nameAttr.DisplayName))
				{
					Name = nameAttr.DisplayName;
				}
				else
				{
					Name = property.Name;
				}
			}

			var browseAttr = property.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
			if (browseAttr != null && !browseAttr.Browsable)
			{
				// option should not be configurable
				IsConfigurable = false;
			}

			if (property.Name == "BeautifyOnSave")
			{
				// option not yet supported in Xamarin Studio
				IsConfigurable = false;
			}

			Description = descAttr.Description;
			Category = categoryAttr.Category;
			PropertyType = property.PropertyType;
			Property = property;
		}
	}
}

