using System;
using System.Text.RegularExpressions;

namespace gw2lav.ViewModel {

	class SettingsViewModel : BindableBase, IDialogViewModel {

		public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

		private Regex _ApiKeyRegex = new Regex(@"^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{20}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$");
		private string _StoredApiKey;
		public bool ApiKeyChanged {
			get { return _ApiKey != _StoredApiKey; }
		}

		private string _ApiKey;
		public string ApiKey {
			get { return _ApiKey; }
			set {
				if (SetProperty(ref _ApiKey, value))
					ApplyCommand.RaiseCanExecuteChanged();
			}
		}

		public RelayCommand ApplyCommand { get; }
		public RelayCommand CancelCommand { get; }

		private bool _NoWater;

		private bool _StoredNoWater;
		public bool NoWater {
			get { return _NoWater; }
			set {
				if (SetProperty(ref _NoWater, value))
					ApplyCommand.RaiseCanExecuteChanged();
			}
		}

		public SettingsViewModel() {
			ApplyCommand = new RelayCommand(OnApply, CanApply);
			CancelCommand = new RelayCommand(OnCancel);

			_StoredApiKey = RegistryHelper.GetApiKey();
			ApiKey = _StoredApiKey;

			_StoredNoWater = RegistryHelper.GetNoWater();
			NoWater = _StoredNoWater;
		}

		private void OnApply() {
			if (_StoredApiKey != ApiKey) {
				RegistryHelper.SetApiKey(ApiKey);
			}

			if (_StoredNoWater != NoWater) {
				RegistryHelper.SetNoWater(NoWater);
			}

			// close dialog
			CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
		}

		private bool CanApply() {
			bool canApply = false;

			if (ApiKey != null && ApiKey != _StoredApiKey) {
				canApply = true;
				if (!_ApiKeyRegex.IsMatch(ApiKey))
					return false;
			}

			if (NoWater != _StoredNoWater) {
				canApply = true;
			}

			return canApply;
		}

		private void OnCancel() {
			// close dialog
			CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
		}

	}

}
