using gw2lav.Model;
using gw2lav.Properties;
using System;
using System.Threading.Tasks;

namespace gw2lav.ViewModel {

	class LegendaryViewModel : BindableBase {

		private LegendaryType[] _LegendaryTypes;
		public LegendaryType[] LegendaryTypes {
			get { return _LegendaryTypes; }
			set { SetProperty(ref _LegendaryTypes, value); }
		}

		private bool _ShowContent;
		public bool ShowContent {
			get { return _ShowContent; }
			set { SetProperty(ref _ShowContent, value); }
		}

		private bool _IsLoading;
		public bool IsLoading {
			get { return _IsLoading; }
			set {
				if (SetProperty(ref _IsLoading, value))
					if (ReloadCommand != null)
						ReloadCommand.RaiseCanExecuteChanged();
			}
		}

		private string _Error;
		public string Error {
			get { return _Error; }
			set { SetProperty(ref _Error, value); }
		}

		public LAVCommand ReloadCommand { get; set; }

		public LAVCommand SettingsCommand { get; set; }

		public LegendaryViewModel() {
			ShowContent = false;
			IsLoading = false;
			Error = null;
			ReloadCommand = new LAVCommand(OnReloadAsync, CanReload);
			SettingsCommand = new LAVCommand(OnSettings, null);
		}

		public async Task LoadDataAsync() {
			// show loading
			ShowContent = false;
			IsLoading = true;
			Error = null;

			// load data
			LegendaryType[] types = null;
			try { types = await GetLegendaryTypesAsync(); } catch (Exception) { }
			if (types != null) {
				LegendaryTypes = types;
			} else {
				// show error
				IsLoading = false;
				Error = R.main_error;
				return;
			}

			// show content
			IsLoading = false;
			ShowContent = true;
		}

		private async Task<LegendaryType[]> GetLegendaryTypesAsync() {
			return await Task.Run(async() => {

				int size = Enum.GetNames(typeof(LegendaryItem.ItemType)).Length - 1;    // "-1" for ItemType.Unknown
				LegendaryType[] result = new LegendaryType[size];
				for (int i = 0; i < size; i++)
					result[i] = new LegendaryType((LegendaryItem.ItemType)i);

				using (ApiHelper apiHelper = new ApiHelper()) {

					// load legendary item list
					Item[] items = await apiHelper.GetLegendaryItemsAsync();
					if (items == null)
						return null;

					// load legendary item counts from account
					CountItem[] countItems = await apiHelper.GetLegendaryItemCountsAsync();
					if (countItems == null) countItems = Array.Empty<CountItem>();

					// sort items to groups
					foreach (Item item in items) {
						CountItem countItem = Array.Find(countItems, ci => ci.Id == item.Id);
						LegendaryItem legendaryItem = new LegendaryItem(item, countItem != null ? countItem.Count : 0);
						if (legendaryItem.Type != LegendaryItem.ItemType.Unknown) {
							result[(int)legendaryItem.Type].Items.Add(legendaryItem);
							result[(int)legendaryItem.Type].recountItems();
						}
					}

				}

				return result;

			});
		}

		private async void OnReloadAsync() {
			await LoadDataAsync();
		}

		private bool CanReload() {
			return !IsLoading;
		}

		private void OnSettings() {
			DialogService.ShowSettings();
		}

	}

}
