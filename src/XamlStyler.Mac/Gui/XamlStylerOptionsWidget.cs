// © Xavalon. All rights reserved.

using Gdk;
using Gtk;
using System;
using System.ComponentModel;
using System.Linq;
using Xavalon.XamlStyler.Mac.ViewModels;

namespace Xavalon.XamlStyler.Mac.Gui
{
    [ToolboxItem(true)]
    public partial class XamlStylerOptionsWidget : Bin
    {
        private readonly XamlStylerOptionsViewModel _viewModel;
        private readonly Color _groupHeaderColor;
        private readonly Color _altRowColor;

        public XamlStylerOptionsWidget(XamlStylerOptionsViewModel viewModel)
        {
            _viewModel = viewModel;

            if (MonoDevelop.Ide.IdeApp.Preferences.UserInterfaceTheme == MonoDevelop.Ide.Theme.Dark)
            {
                _groupHeaderColor = Colors.MineShaft;
                _altRowColor = Colors.Tundora;
            }
            else
            {
                _groupHeaderColor = Colors.SilverChalice;
                _altRowColor = Colors.Gallery;
            }

            InitializeUI();
        }

        protected void OnResetButtonClicked(object sender, EventArgs e)
        {
            _viewModel.ResetToDefaults();

            foreach (var item in SettingsTable.Children)
            {
                item.Destroy();
            }

            InitializeSettingsTable();
        }

        private void InitializeUI()
        {
            Build();
            InitializeSettingsTable();
        }

        private void InitializeSettingsTable()
        {
            _viewModel.RefreshData();

            var rowsCount = _viewModel.GroupedOptions.Sum(group => group.Count() + 1);
            SettingsTable.NRows = (uint)rowsCount;

            var currentRow = (uint)0;

            foreach (var optionGroup in _viewModel.GroupedOptions)
            {
                var groupLabel = new Label
                {
                    HeightRequest = 40,
                    Markup = $"<b>{optionGroup.Key}</b>"
                };
                groupLabel.SetPadding(5, 0);
                groupLabel.SetAlignment(0f, 0.5f);

                var box = new EventBox { groupLabel };
                box.ModifyBg(StateType.Normal, _groupHeaderColor);

                SettingsTable.Attach(box, 0, 2, currentRow, currentRow + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
                currentRow++;

                foreach (var option in optionGroup)
                {
                    // name label
                    var lbl = new Label(option.Name) { TooltipText = option.Description };
                    lbl.SetAlignment(0f, 0.5f);
                    lbl.HeightRequest = 30;
                    AddToTable(currentRow, 0, lbl);

                    var type = option.PropertyType;
                    if (type == typeof(bool))
                    {
                        var chk = new CheckButton { Active = (bool)option.Property.GetValue(_viewModel.Options) };
                        chk.Clicked += (sender, e) =>
                        {
                            option.Property.SetValue(_viewModel.Options, chk.Active);
                            _viewModel.IsDirty = true;
                        };
                        AddToTable(currentRow, 1, chk);
                    }
                    else if (type == typeof(int))
                    {
                        var val = (int)option.Property.GetValue(_viewModel.Options);
                        var spin = new SpinButton(0, 10, 1) { Value = val, WidthChars = 3 };
                        spin.ValueChanged += (sender, e) =>
                        {
                            option.Property.SetValue(_viewModel.Options, (int)spin.Value);
                            _viewModel.IsDirty = true;
                        };
                        AddToTable(currentRow, 1, spin);
                    }
                    else if (type == typeof(byte))
                    {
                        var val = (byte)option.Property.GetValue(_viewModel.Options);
                        var spin = new SpinButton(0, 10, 1) { Value = val, WidthChars = 3 };
                        spin.ValueChanged += (sender, e) =>
                        {
                            option.Property.SetValue(_viewModel.Options, (byte)spin.Value);
                            _viewModel.IsDirty = true;
                        };
                        AddToTable(currentRow, 1, spin);
                    }
                    else if (type == typeof(string))
                    {
                        var val = (string)option.Property.GetValue(_viewModel.Options);
                        var txt = new Entry(val);
                        txt.Alignment = 0;
                        txt.Changed += (sender, e) =>
                        {
                            option.Property.SetValue(_viewModel.Options, txt.Text);
                            _viewModel.IsDirty = true;
                        };
                        AddToTable(currentRow, 1, txt);
                    }
                    else if (type == typeof(string[]))
                    {
                        var vals = (string[])option.Property.GetValue(_viewModel.Options);
                        var val = string.Join(Environment.NewLine, vals);
                        var txt = new TextView(new TextBuffer(new TextTagTable()))
                        {
                            LeftMargin = 5,
                            RightMargin = 5,
                            BorderWidth = 1
                        };
                        txt.SetSizeRequest(320, 200);
                        txt.Buffer.Text = val;
                        txt.Buffer.Changed += (sender, e) =>
                        {
                            var newVals = txt.Buffer.Text.Split(Environment.NewLine.ToCharArray());
                            option.Property.SetValue(_viewModel.Options, newVals);
                            _viewModel.IsDirty = true;
                        };

                        var frame = new Frame
                        {
                            Shadow = ShadowType.In,
                            BorderWidth = 5,
                            Child = txt
                        };

                        AddToTable(currentRow, 1, frame);
                    }
                    else if (type.IsEnum)
                    {
                        var val = option.Property.GetValue(_viewModel.Options);
                        var values = Enum.GetNames(type);
                        var cmb = new ComboBox(values)
                        {
                            Active = Array.IndexOf(values, val.ToString())
                        };

                        cmb.Changed += (sender, e) =>
                        {
                            option.Property.SetValue(_viewModel.Options, Enum.Parse(type, cmb.ActiveText));
                            _viewModel.IsDirty = true;
                        };

                        AddToTable(currentRow, 1, cmb);
                    }

                    ++currentRow;
                }
            }

            ShowAll();
        }

        private void AddToTable(uint row, uint column, Widget control, uint columnSpan = 1)
        {
            var alignmentXScale = control is Entry ? 1 : 0;
            var alignment = new Alignment(0, 0, alignmentXScale, 1) { control };
            alignment.SetPadding(0, 0, 5, 5);

            var box = new EventBox { alignment };
            if (row % 2 is 1)
            {
                box.ModifyBg(StateType.Normal, _altRowColor);
            }

            SettingsTable.Attach(box, column, column + columnSpan, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
        }
    }
}