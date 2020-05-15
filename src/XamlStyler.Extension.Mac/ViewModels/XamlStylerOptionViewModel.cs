// (c) Xavalon. All rights reserved.

using System;
using System.ComponentModel;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Extension.Mac.ViewModels
{
    public class XamlStylerOptionViewModel
    {
        public XamlStylerOptionViewModel(PropertyDescriptor property)
        {
            var browsableAttribute = (BrowsableAttribute)property.Attributes[typeof(BrowsableAttribute)];
            var displayNameAttribute = (DisplayNameAttribute)property.Attributes[typeof(DisplayNameAttribute)];
            var descriptionAttribute = (DescriptionAttribute)property.Attributes[typeof(DescriptionAttribute)];
            var categoryAttribute = (CategoryAttribute)property.Attributes[typeof(CategoryAttribute)];

            var name = displayNameAttribute?.DisplayName ?? property.Name;
            var isConfigurable = browsableAttribute is null || browsableAttribute.Browsable;
            AdjustItems(property, ref isConfigurable, ref name);

            Name = name;
            IsConfigurable = isConfigurable;
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

        // TODO Unify Mac Extension approach with one that we have in Windows Extension
        // Currently we not handling correctly Options changes as well as we not loading options from Visual Studio.
        // This method is workaround for now. Remove this workaround when options loading will be implemented
        private void AdjustItems(PropertyDescriptor property, ref bool isConfigurable, ref string name)
        {
            switch (property.Name)
            {
                case nameof(IStylerOptions.ResetToDefault):
                    isConfigurable = false;
                    break;
                case nameof(IStylerOptions.IndentWithTabs):
                    isConfigurable = true;
                    name = "Indent with tabs";
                    break;
            }
        }
    }
}