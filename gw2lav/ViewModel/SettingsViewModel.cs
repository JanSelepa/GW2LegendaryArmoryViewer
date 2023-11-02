using gw2lav.Service;
using System;
using System.Text.RegularExpressions;

namespace gw2lav.ViewModel {

	class SettingsViewModel : BindableBase, IDialogViewModel {

		private IRegistryService _RegistryService;

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

		private bool _NoInventory;

		private bool _StoredNoInventory;
		public bool NoInventory {
			get { return _NoInventory; }
			set {
				if (SetProperty(ref _NoInventory, value))
					ApplyCommand.RaiseCanExecuteChanged();
			}
		}

		public SettingsViewModel(IRegistryService registryService) {
			_RegistryService = registryService;

			ApplyCommand = new RelayCommand(OnApply, CanApply);
			CancelCommand = new RelayCommand(OnCancel);

			_StoredApiKey = _RegistryService.GetApiKey();
			ApiKey = _StoredApiKey;

			_StoredNoWater = _RegistryService.GetNoWater();
			NoWater = _StoredNoWater;

			_StoredNoInventory = _RegistryService.GetNoInventory();
			NoInventory = _StoredNoInventory;
		}

		private void OnApply() {
			if (_StoredApiKey != ApiKey) {
				_RegistryService.SetApiKey(ApiKey);
			}

			if (_StoredNoWater != NoWater) {
				_RegistryService.SetNoWater(NoWater);
			}

			if (_StoredNoInventory != NoInventory) {
				_RegistryService.SetNoInventory(NoInventory);
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

			if (NoInventory != _StoredNoInventory) {
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
