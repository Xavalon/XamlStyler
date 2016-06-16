
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.XamarinStudio
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class OptionsWidget : Gtk.Bin
	{
		public OptionsWidget()
		{
			this.Build();
			this.InitializeWidget();
		}

		void InitializeWidget()
		{
			var options = new StylerOptions();

			var optionsList = new List<Option>();
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(options))
			{
				optionsList.Add(new Option(property));
			}

			var groupedOptions = optionsList.GroupBy(x => x.Category).ToList();

			// one row per option and one per group
			tblContainer.NRows = (uint)(groupedOptions.Sum(x => x.Count()) + groupedOptions.Count());
			tblContainer.NColumns = 2;
			tblContainer.RowSpacing = 0;
			tblContainer.ColumnSpacing = 0;
			tblContainer.BorderWidth = 0;

			uint r = 0;
			foreach (var optionGroup in groupedOptions)
			{
				// group label
				var grouplbl = new Gtk.Label();
				grouplbl.SetAlignment(0f, 0.5f);
				grouplbl.Markup = $"<b> {optionGroup.Key}</b>";

				var box = new Gtk.EventBox();
				box.ModifyBg(Gtk.StateType.Normal, new Gdk.Color(0xaa, 0xaa, 0xaa));
				box.Add(grouplbl);
				tblContainer.Attach(box, 0, 2, r, r + 1, Gtk.AttachOptions.Fill, Gtk.AttachOptions.Fill, 0, 0);
				r++;

				foreach (var option in optionGroup)
				{
					// name label
					var lbl = new Gtk.Label(option.Name) { TooltipText = option.Description };
					lbl.SetAlignment(0f, 0.5f);
					AddToTable(r, 0, lbl);

					var type = option.PropertyType;
					if (type == typeof(bool))
					{
						AddToTable(r, 1, new Gtk.CheckButton { Active = (bool)option.Property.GetValue(options) });
					}
					else if (type == typeof(int))
					{
						var val = (int)option.Property.GetValue(options);
						AddToTable(r, 1, new Gtk.SpinButton(0, 10, 1) { Value = val, WidthChars = 3 });
					}
					else if (type == typeof(byte))
					{
						var val = (byte)option.Property.GetValue(options);
						AddToTable(r, 1, new Gtk.SpinButton(0, 10, 1) { Value = val, WidthChars = 3 });
					}
					else if (type == typeof(string))
					{
						var val = (string)option.Property.GetValue(options);
						var txt = new Gtk.Entry(val);
						txt.Alignment = 0;
						AddToTable(r, 1, new Gtk.Entry(val));
					}
					else if (type.IsEnum)
					{
						var val = option.Property.GetValue(options);
						var values = Enum.GetNames(type);
						var cmb = new Gtk.ComboBox(values);
						cmb.Active = Array.IndexOf(values, val.ToString());
						AddToTable(r, 1, cmb);
					}

					r++;
				}
			}

			//tblContainer.ShowAll();
			alContainer.ShowAll();
		}

		private void AddToTable(uint row, uint col, Gtk.Widget control, uint colSpan = 1)
		{
			var box = new Gtk.EventBox();
			var al = new Gtk.Alignment(0, 0, control.GetType() == typeof(Gtk.Entry) ? 1 : 0, 1);
			al.SetPadding(0, 0, 5, 5);
			al.Add(control);
			box.Add(al);

			if (row % 2 == 1)
			{
				box.ModifyBg(Gtk.StateType.Normal, new Gdk.Color(0xee, 0xee, 0xee));
			}

			tblContainer.Attach(box, col, col + colSpan, row, row + 1, Gtk.AttachOptions.Fill, Gtk.AttachOptions.Fill, 0, 0);
		}

		private class Option
		{
			public string Name { get; set; }

			public string Description { get; set; }

			public string Category { get; set; }

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

				Description = descAttr.Description;
				Category = categoryAttr.Category;
				PropertyType = property.PropertyType;
				Property = property;
			}
		}
	}
}

