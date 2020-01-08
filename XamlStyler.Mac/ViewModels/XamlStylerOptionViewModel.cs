using System;
using System.ComponentModel;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Mac.ViewModels
{
    public class XamlStylerOptionViewModel
    {
        public XamlStylerOptionViewModel(PropertyDescriptor property)
        {
            var browsableAttribute = (BrowsableAttribute)property.Attributes[typeof(BrowsableAttribute)];
            var displayNameAttribute = (DisplayNameAttribute)property.Attributes[typeof(DisplayNameAttribute)];
            var descriptionAttribute = (DescriptionAttribute)property.Attributes[typeof(DescriptionAttribute)];
            var categoryAttribute = (CategoryAttribute)property.Attributes[typeof(CategoryAttribute)];

            IsConfigurable = browsableAttribute is null || browsableAttribute.Browsable;

            // TODO Discuss if ResetToDefault should be configurable by default, it looks strange
            IsConfigurable &= property.Name != nameof(IStylerOptions.ResetToDefault);

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