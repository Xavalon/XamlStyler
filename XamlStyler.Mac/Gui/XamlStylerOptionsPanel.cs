using MonoDevelop.Components;
using MonoDevelop.Ide.Gui.Dialogs;

namespace Xavalon.XamlStyler.Mac.Gui
{
	public class XamlStylerOptionsPanel : OptionsPanel
	{
        private readonly OptionsViewModel _optionsViewModel;

        public XamlStylerOptionsPanel()
        {
            _optionsViewModel = new OptionsViewModel();
        }

		public override Control CreatePanelWidget()
		{
			return new OptionsWidget(_optionsViewModel);
		}

		public override void ApplyChanges()
		{
			_optionsViewModel.SaveOptions();
		}
	}
}