using MonoDevelop.Ide.Gui.Dialogs;

namespace Xavalon.XamlStyler.XamarinStudio.Gui
{
	public class XamlStylerOptionsPanel : OptionsPanel
	{
		private OptionsViewModel _optionsViewModel = new OptionsViewModel();

		public override MonoDevelop.Components.Control CreatePanelWidget()
		{
			return new OptionsWidget(_optionsViewModel);
		}

		public override void ApplyChanges()
		{
			_optionsViewModel.SaveOptions();
		}
	}
}