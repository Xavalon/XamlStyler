using System;
using System.ComponentModel;

namespace Xavalon.XamlStyler.Mac.Gui
{
	public class OptionViewModel
	{
		public OptionViewModel(PropertyDescriptor property)
		{
            var browsableAttribute = (BrowsableAttribute)property.Attributes[typeof(BrowsableAttribute)];
            var displayNameAttribute = (DisplayNameAttribute)property.Attributes[typeof(DisplayNameAttribute)];
            var descriptionAttribute = (DescriptionAttribute)property.Attributes[typeof(DescriptionAttribute)];
            var categoryAttribute = (CategoryAttribute)property.Attributes[typeof(CategoryAttribute)];

            IsConfigurable = browsableAttribute is null || browsableAttribute.Browsable;
            Name = displayNameAttribute?.DisplayName ?? property.Name;
			Description = descriptionAttribute.Description;
			Category = categoryAttribute.Category;
			PropertyType = property.PropertyType;
			Property = property;
		}

        public string Name { get; }

        public string Description { get; }

        public string Category { get; }

        public bool IsConfigurable { get; }

        public Type PropertyType { get; }

        public PropertyDescriptor Property { get; }
    }
}

