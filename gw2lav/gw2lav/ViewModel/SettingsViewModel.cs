using System.Text.RegularExpressions;

namespace gw2lav.ViewModel {

	class SettingsViewModel : BindableBase {

		private Regex _ApiKeyRegex = new Regex(@"^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{20}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$");
		private string _StoredApiKey;

		private string _ApiKey;
		public string ApiKey {
			get { return _ApiKey; }
			set {
				if (SetProperty(ref _ApiKey, value))
					if (ApplyApiKeyCommand != null)
						ApplyApiKeyCommand.RaiseCanExecuteChanged();
			}
		}

		public LAVCommand ApplyApiKeyCommand { get; set; }

		public SettingsViewModel() {
			ApplyApiKeyCommand = new LAVCommand(OnApplyApiKey, CanApplyApiKey);

			_StoredApiKey = RegistryHelper.getApiKey();
			ApiKey = _StoredApiKey;
		}

		private void OnApplyApiKey() {
			RegistryHelper.setApiKey(ApiKey);
			_StoredApiKey = ApiKey;
			if (ApplyApiKeyCommand != null)
				ApplyApiKeyCommand.RaiseCanExecuteChanged();
			// TODO reload
		}

		private bool CanApplyApiKey() {
			if (ApiKey == null) return false;
			if (ApiKey == _StoredApiKey) return false;
			return _ApiKeyRegex.IsMatch(ApiKey);
		}

	}

}
