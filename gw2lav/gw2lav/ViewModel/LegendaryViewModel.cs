using gw2lav.Model;
using gw2lav.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace gw2lav.ViewModel {

	class LegendaryViewModel : BindableBase {

		private class ItemInfo {
			public int ItemId;
			public string CharName;
			public string TabName;
			public int TabId;
			public ItemInfo(int itemId, string charName, string tabName, int tabId) {
				ItemId = itemId;
				CharName = charName;
				TabName = tabName;
				TabId = tabId;
			}
		}

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
			// loading started
			ShowContent = true;
			IsLoading = true;
			Error = null;

			// load data
			try {
				await LoadLegendaryTypesAsync();
			} catch (Exception) {
				// show error
				ShowContent = false;
				IsLoading = false;
				Error = R.main_error;
				return;
			}

			// loading finished
			IsLoading = false;
		}

		private async Task LoadLegendaryTypesAsync() {
			await Task.Run(async () => {

				int size = Enum.GetNames(typeof(LegendaryItem.ItemType)).Length - 1;    // "-1" for ItemType.Unknown
				LegendaryType[] types = new LegendaryType[size];
				for (int i = 0; i < size; i++)
					types[i] = new LegendaryType((LegendaryItem.ItemType)i);
				LegendaryTypes = types;

				using (ApiHelper apiHelper = new ApiHelper()) {

					// load legendary item list
					Item[] items = await apiHelper.GetLegendaryItemsAsync();
					if (items == null)
						throw new Exception();

					// load legendary item counts from account
					CountItem[] countItems = await apiHelper.GetLegendaryItemCountsAsync();
					if (countItems == null) countItems = Array.Empty<CountItem>();

					// sort items to groups
					foreach (Item item in items) {
						CountItem countItem = Array.Find(countItems, ci => ci.Id == item.Id);
						LegendaryItem legendaryItem = new LegendaryItem(item, countItem != null ? countItem.Count : 0);
						if (legendaryItem.Type != LegendaryItem.ItemType.Unknown) {
							await Application.Current.Dispatcher.BeginInvoke(new Action(() => LegendaryTypes[(int)legendaryItem.Type].Items.Add(legendaryItem)));
							LegendaryTypes[(int)legendaryItem.Type].recountItems();
						}
					}

					// get info about items replacable by legendaries
					Character[] characters = await apiHelper.GetCharactersAsync();
					if (characters != null) {
						// get items that can be replaced by legendaries
						List<ItemInfo> potentials = new List<ItemInfo>();
						foreach (Character ch in characters) {
							if (ch.Level != 80) continue;
							foreach (EquipmentTab et in ch.EquipmentTabs) {
								foreach (Equipment eq in et.Equipment) {
									// check upgrade component
									if ((
											eq.Slot == Equipment.SlotType.HelmAquatic || eq.Slot == Equipment.SlotType.WeaponAquaticA || eq.Slot == Equipment.SlotType.WeaponAquaticB
											|| eq.Slot == Equipment.SlotType.Helm || eq.Slot == Equipment.SlotType.Shoulders || eq.Slot == Equipment.SlotType.Gloves || eq.Slot == Equipment.SlotType.Coat || eq.Slot == Equipment.SlotType.Leggings || eq.Slot == Equipment.SlotType.Boots
											|| eq.Slot == Equipment.SlotType.WeaponA1 || eq.Slot == Equipment.SlotType.WeaponA2 || eq.Slot == Equipment.SlotType.WeaponB1 || eq.Slot == Equipment.SlotType.WeaponB2
										) && eq.Upgrades != null) {
										foreach (int upgradeId in eq.Upgrades) {
											potentials.Add(new ItemInfo(upgradeId, ch.Name, et.Name, et.Tab));
										}
									}
									// skip aquatic helm
									if (eq.Slot == Equipment.SlotType.HelmAquatic)
										continue;
									// skip items that are already legendary
									if (eq.Location == Equipment.LocationType.LegendaryArmory || eq.Location == Equipment.LocationType.EquippedFromLegendaryArmory)
										continue;
									// add item to potentially wanted items
									potentials.Add(new ItemInfo(eq.Id, ch.Name, et.Name, et.Tab));
								}
							}
						}
						if (potentials.Count > 0) {
							// get unique item ids to check for item type
							string ids = string.Join(",", potentials.Select(p => p.ItemId).Distinct());
							// get item info about items replacable by legendaries to get the proper item type
							Item[] replacableItems = await apiHelper.GetItemsAsync(ids);
							if (replacableItems != null) {
								foreach (Item item in replacableItems) {
									// skip legendary items (mostly used for upgrades)
									if (item.Rarity == Item.ItemRarity.Legendary)
										continue;
									// skip unknown types
									LegendaryItem.ItemType itemType = LegendaryItem.GetItemType(item);
									if (itemType == LegendaryItem.ItemType.Unknown)
										continue;
									// count all items from a type
									List<ItemInfo> potentialsWithSameId = potentials.FindAll(p => p.ItemId == item.Id);
									foreach (ItemInfo p in potentialsWithSameId) {
										LegendaryTypes[(int)itemType].WantedInfo.Add(p.CharName, p.TabName, p.TabId);
									}
								}
							}
						}
					}
				}

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
