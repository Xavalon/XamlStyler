
// This file has been generated by the GUI designer. Do not modify.
namespace Xavalon.XamlStyler.Mac.Gui
{
	public partial class XamlStylerOptionsWidget
	{
		private global::Gtk.VBox VBoxContainer;

		private global::Gtk.Button ResetButton;

		private global::Gtk.ScrolledWindow ScrollContainer;

		private global::Gtk.Table SettingsTable;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget Xavalon.XamlStyler.Mac.Gui.XamlStylerOptionsWidget
			global::Stetic.BinContainer.Attach(this);
			this.Name = "Xavalon.XamlStyler.Mac.Gui.XamlStylerOptionsWidget";
			// Container child Xavalon.XamlStyler.Mac.Gui.XamlStylerOptionsWidget.Gtk.Container+ContainerChild
			this.VBoxContainer = new global::Gtk.VBox();
			this.VBoxContainer.Name = "VBoxContainer";
			this.VBoxContainer.Spacing = 6;
			// Container child VBoxContainer.Gtk.Box+BoxChild
			this.ResetButton = new global::Gtk.Button();
			this.ResetButton.CanFocus = true;
			this.ResetButton.Name = "ResetButton";
			this.ResetButton.UseUnderline = true;
			this.ResetButton.FocusOnClick = false;
			this.ResetButton.Label = global::Mono.Unix.Catalog.GetString("Reset to defaults");
			this.VBoxContainer.Add(this.ResetButton);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.VBoxContainer[this.ResetButton]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child VBoxContainer.Gtk.Box+BoxChild
			this.ScrollContainer = new global::Gtk.ScrolledWindow();
			this.ScrollContainer.Name = "ScrollContainer";
			// Container child ScrollContainer.Gtk.Container+ContainerChild
			global::Gtk.Viewport w2 = new global::Gtk.Viewport();
			w2.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child GtkViewport.Gtk.Container+ContainerChild
			this.SettingsTable = new global::Gtk.Table(((uint)(1)), ((uint)(2)), false);
			this.SettingsTable.Name = "SettingsTable";
			w2.Add(this.SettingsTable);
			this.ScrollContainer.Add(w2);
			this.VBoxContainer.Add(this.ScrollContainer);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.VBoxContainer[this.ScrollContainer]));
			w5.Position = 1;
			this.Add(this.VBoxContainer);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
			this.ResetButton.Clicked += new global::System.EventHandler(this.OnResetButtonClicked);
		}
	}
}
