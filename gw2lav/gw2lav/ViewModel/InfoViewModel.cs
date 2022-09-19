using System;
using System.Reflection;

namespace gw2lav.ViewModel {

	class InfoViewModel : BindableBase, IDialogViewModel {

		public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

		public string Version { get; set; }

		public InfoViewModel() {
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			Version = version.Major + "." + version.Minor + "." + version.Build;
		}

	}

}
