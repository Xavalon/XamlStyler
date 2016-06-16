using System;
using MonoDevelop.Ide.Gui.Dialogs;

namespace Xavalon.XamlStyler.XamarinStudio.Interface
{
	public class XamlStylerOptionsPanel : OptionsPanel
	{
		//PackageManagementOptionsViewModel optionsViewModel;

		public override MonoDevelop.Components.Control CreatePanelWidget()
		{
			return new OptionsWidget();
		}

		public override void ApplyChanges()
		{
			//optionsViewModel.SaveOptions();
		}
	}
}