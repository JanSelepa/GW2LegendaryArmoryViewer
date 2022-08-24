using gw2lav.Model;
using gw2lav.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
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

					// get info about items replacable by legendaries
					Character[] characters = await apiHelper.GetCharactersAsync();
					if (characters != null) {
						// get items that can be replaced by legendaries
						Dictionary<int, WantedInfo> potentials = new Dictionary<int, WantedInfo>();
						foreach (Character ch in characters) {
							if (ch.Level != 80) continue;
							foreach (EquipmentTab et in ch.EquipmentTabs) {
								foreach (Equipment eq in et.Equipment) {
									// skip aquatic helm
									if (eq.Slot == Equipment.SlotType.HelmAquatic)
										continue;
									// skip items that are already legendary
									if (eq.Location == Equipment.LocationType.LegendaryArmory || eq.Location == Equipment.LocationType.EquippedFromLegendaryArmory)
										continue;
									// add item to potentially wanted items
									WantedInfo wanted = null;
									try {
										wanted = potentials[eq.Id];
									} catch (KeyNotFoundException) {
										wanted = new WantedInfo();
										potentials.Add(eq.Id, wanted);

									}
									wanted.Add(ch.Name, et.Name, et.Tab);
								}
							}
						}
						// get item info about items replacable by legendaries to get the proper item type
						Item[] replacableItems = await apiHelper.GetItemsAsync(string.Join(",", potentials.Keys.ToArray()));
						foreach (Item item in replacableItems) {
							LegendaryItem.ItemType itemType = LegendaryItem.GetItemType(item);
							if (itemType == LegendaryItem.ItemType.Unknown)
								continue;
							result[(int)itemType].WantedInfo.Add(potentials[item.Id]);
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
