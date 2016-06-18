using System;
using System.Linq;

namespace Xavalon.XamlStyler.XamarinStudio.Gui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class OptionsWidget : Gtk.Bin
	{
		private OptionsViewModel _viewModel;

		public OptionsWidget(OptionsViewModel viewModel)
		{
			_viewModel = viewModel;

			this.Build();
			this.InitializeWidget();
		}

		void InitializeWidget()
		{
			_viewModel.Init();

			// one row per option and one per group
			tblContainer.NRows = (uint)(_viewModel.GroupedOptions.Sum(x => x.Count()) + _viewModel.GroupedOptions.Count()) + 1;
			tblContainer.NColumns = 2;
			tblContainer.RowSpacing = 0;
			tblContainer.ColumnSpacing = 0;
			tblContainer.BorderWidth = 0;

			uint r = 0;

			var btn = new Gtk.Button();
			btn.Label = "Reset to defaults";
			btn.WidthRequest = 150;
			btn.Clicked += (sender, e) =>
			{
				_viewModel.ResetToDefaults();

				foreach (var item in tblContainer.Children)
				{
					item.Destroy();
				}

				InitializeWidget();
			};
			tblContainer.Attach(btn, 0, 2, r, r + 1, Gtk.AttachOptions.Fill, Gtk.AttachOptions.Fill, 0, 0);
			r++;

			foreach (var optionGroup in _viewModel.GroupedOptions)
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
						var chk = new Gtk.CheckButton { Active = (bool)option.Property.GetValue(_viewModel.Options) };
						chk.Clicked += (sender, e) =>
						{
							option.Property.SetValue(_viewModel.Options, chk.Active);
						};
						AddToTable(r, 1, chk);
					}
					else if (type == typeof(int))
					{
						var val = (int)option.Property.GetValue(_viewModel.Options);
						var spin = new Gtk.SpinButton(0, 10, 1) { Value = val, WidthChars = 3 };
						spin.ValueChanged += (sender, e) =>
						{
							option.Property.SetValue(_viewModel.Options, (int)spin.Value);
						};
						AddToTable(r, 1, spin);
					}
					else if (type == typeof(byte))
					{
						var val = (byte)option.Property.GetValue(_viewModel.Options);
						var spin = new Gtk.SpinButton(0, 10, 1) { Value = val, WidthChars = 3 };
						spin.ValueChanged += (sender, e) =>
						{
							option.Property.SetValue(_viewModel.Options, (int)spin.Value);
						};
						AddToTable(r, 1, spin);
					}
					else if (type == typeof(string))
					{
						var val = (string)option.Property.GetValue(_viewModel.Options);
						var txt = new Gtk.Entry(val);
						txt.Alignment = 0;
						txt.Changed += (sender, e) =>
						{
							option.Property.SetValue(_viewModel.Options, txt.Text);
						};
						AddToTable(r, 1, txt);
					}
					else if (type.IsEnum)
					{
						var val = option.Property.GetValue(_viewModel.Options);
						var values = Enum.GetNames(type);
						var cmb = new Gtk.ComboBox(values);
						cmb.Active = Array.IndexOf(values, val.ToString());
						cmb.Changed += (sender, e) =>
						{
							option.Property.SetValue(_viewModel.Options, Enum.Parse(type, cmb.ActiveText));
						};
						AddToTable(r, 1, cmb);
					}

					r++;
				}
			}

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
	}
}

