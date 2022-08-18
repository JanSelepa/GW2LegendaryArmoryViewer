using gw2lav.Properties;
using gw2lav.View;

namespace gw2lav.ViewModel {

	class DialogService {

		public static void ShowSettings() {
			DialogWindow dlg = new DialogWindow();
			dlg.Title = R.settings_title;
			dlg.SetContent(new SettingsControl());
			dlg.ShowDialog();
		}

	}

}
