using MonoDevelop.Components;
using MonoDevelop.Ide.Gui.Dialogs;
using Xavalon.XamlStyler.Mac.ViewModels;

namespace Xavalon.XamlStyler.Mac.Gui
{
	public class XamlStylerOptionsPanel : OptionsPanel
	{
        public XamlStylerOptionsPanel()
        {
            ViewModel = new XamlStylerOptionsViewModel();
        }

        private XamlStylerOptionsViewModel ViewModel { get; }

		public override Control CreatePanelWidget()
		{
			return new XamlStylerOptionsWidget(ViewModel);
		}

		public override void ApplyChanges()
		{
			ViewModel.SaveOptions();
		}
	}
}