using gw2lav.Properties;
using gw2lav.Service;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace gw2lav.ViewModel {

	class InfoViewModel : BindableBase, IDialogViewModel {

		public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

		public string Version { get; set; }

		private bool _IsUpdateInProgress;
		public bool IsUpdateInProgress {
			get { return _IsUpdateInProgress; }
			set {
				SetProperty(ref _IsUpdateInProgress, value);
				if (UpdateCommand != null)
					UpdateCommand.RaiseCanExecuteChanged();
			}
		}

		private bool _IsUpdateAvailable;
		public bool IsUpdateAvailable {
			get { return _IsUpdateAvailable; }
			set { SetProperty(ref _IsUpdateAvailable, value); }
		}

		private string _AvailableVersion;
		public string AvailableVersion {
			get { return _AvailableVersion; }
			set { SetProperty(ref _AvailableVersion, value); }
		}

		public RelayCommand UpdateCommand { get; set; }

		private IDialogService _DialogService;
		private IUpdateService _UpdateService;

		public InfoViewModel(IDialogService dialogService, IUpdateService updateService) {
			_DialogService = dialogService;
			_UpdateService = updateService;
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			Version = version.Major + "." + version.Minor + "." + version.Build;
			IsUpdateInProgress = false;
			AvailableVersion = null;
			IsUpdateAvailable = false;
			UpdateCommand = new RelayCommand(OnUpdateAsync, CanUpdate);

			_ = RetrieveUpdateInfoAsync();
		}

		private async Task RetrieveUpdateInfoAsync() {
			AvailableVersion = await _UpdateService.GetAvailableVersionAsync();
			IsUpdateAvailable = await _UpdateService.IsUpdateAvailableAsync();
		}

		private async void OnUpdateAsync() {
			IsUpdateInProgress = true;
			string updateError = await _UpdateService.UpdateAsync();
			if (updateError != null) {
				MessageViewModel dialogVM = new MessageViewModel(R.info_update_error_title, string.Format(R.info_update_error_message, updateError), R.btn_ok, null);
				_DialogService.ShowDialog(dialogVM);
			}
			IsUpdateInProgress = false;
		}

		private bool CanUpdate() {
			return IsUpdateAvailable && !IsUpdateInProgress;
		}

	}

}
