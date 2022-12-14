using gw2lav.Model;
using gw2lav.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace gw2lav.ViewModel {

	class LegendaryViewModel : BindableBase {

		private class ItemInfo {
			public int ItemId;
			public string CharName;
			public string TabName;
			public int TabId;
			public bool IsTerrestrial;
			public ItemInfo(int itemId, string charName, string tabName, int tabId, bool isTerrestrial) {
				ItemId = itemId;
				CharName = charName;
				TabName = tabName;
				TabId = tabId;
				IsTerrestrial = isTerrestrial;
			}
		}

		private IDialogService _DialogService;
		private IUpdateHelper _UpdateHelper;
		private CancellationTokenSource _CancellationTokenSource = null;

		private LegendaryType[] _LegendaryTypes;
		public LegendaryType[] LegendaryTypes {
			get { return _LegendaryTypes; }
			set { SetProperty(ref _LegendaryTypes, value); }
		}

		private LegendaryType _Detail;
		public LegendaryType Detail {
			get { return _Detail; }
			set { SetProperty(ref _Detail, value); }
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

		private bool _NoWater;
		public bool NoWater {
			get { return _NoWater; }
			set { SetProperty(ref _NoWater, value); }
		}

		private bool _IsDetailLoaded;
		public bool IsDetailLoaded {
			get { return _IsDetailLoaded; }
			set { SetProperty(ref _IsDetailLoaded, value); }
		}

		private bool _IsUpdateAvailable;
		public bool IsUpdateAvailable {
			get { return _IsUpdateAvailable; }
			set { SetProperty(ref _IsUpdateAvailable, value); }
		}

		public RelayCommand ReloadCommand { get; set; }
		public RelayCommand SettingsCommand { get; set; }
		public RelayCommand InfoCommand { get; set; }

		public RelayCommand<LegendaryType> TypeSelectedCommand { get; set; }

		public LegendaryViewModel(IDialogService dialogService, IUpdateHelper updateHelper) {
			_DialogService = dialogService;
			_UpdateHelper = updateHelper;
			ShowContent = false;
			IsLoading = false;
			Error = null;
			NoWater = RegistryHelper.GetNoWater();
			IsDetailLoaded = false;
			IsUpdateAvailable = false;
			ReloadCommand = new RelayCommand(OnReloadAsync, CanReload);
			SettingsCommand = new RelayCommand(OnSettings, null);
			InfoCommand = new RelayCommand(OnInfo, null);
			TypeSelectedCommand = new RelayCommand<LegendaryType>(OnTypeSelected, null);

			_ = CheckForUpdateAsync();
			_ = LoadDataAsync();
		}

		private async Task CheckForUpdateAsync() {
			IsUpdateAvailable = await _UpdateHelper.IsUpdateAvailableAsync();
		}

		private async Task LoadDataAsync() {
			if (_CancellationTokenSource != null) {
				_CancellationTokenSource.Cancel();
				return;
			}

			// loading started
			Detail = null;
			ShowContent = true;
			IsLoading = true;
			Error = null;
			IsDetailLoaded = false;

			// load data
			_CancellationTokenSource = new CancellationTokenSource();
			bool runAgain = false;
			try {
				await LoadLegendaryTypesAsync(_CancellationTokenSource.Token);
			} catch (OperationCanceledException) {
				runAgain = true;
			} catch (Exception) {
				// show error
				ShowContent = false;
				IsLoading = false;
				Error = R.main_error;
				return;
			} finally {
				_CancellationTokenSource.Dispose();
				_CancellationTokenSource = null;
			}

			// loading finished
			IsLoading = false;

			if (runAgain) await LoadDataAsync();
		}

		private async Task LoadLegendaryTypesAsync(CancellationToken cancelToken) {
			await Task.Run(async () => {

				int size = Enum.GetNames(typeof(LegendaryItem.ItemType)).Length - 1;    // "-1" for ItemType.Unknown
				LegendaryType[] types = new LegendaryType[size];
				for (int i = 0; i < size; i++)
					types[i] = new LegendaryType((LegendaryItem.ItemType)i);
				LegendaryTypes = types;

				using (ApiHelper apiHelper = new ApiHelper(cancelToken)) {

					// load legendary item list
					List<Item> items = await apiHelper.GetLegendaryItemsAsync();
					cancelToken.ThrowIfCancellationRequested();
					if (items == null)
						throw new Exception();

					items.Sort((i1, i2) => { return i1.Id.CompareTo(i2.Id); });

					// load legendary item counts from account
					List<CountItem> countItems = await apiHelper.GetLegendaryItemCountsAsync();
					cancelToken.ThrowIfCancellationRequested();
					if (countItems == null) countItems = new List<CountItem>();

					// sort items to groups
					foreach (Item item in items) {
						CountItem countItem = countItems.FirstOrDefault(ci => ci.Id == item.Id);
						LegendaryItem legendaryItem = new LegendaryItem(item, countItem != null ? countItem.Count : 0);
						if (legendaryItem.Type != LegendaryItem.ItemType.Unknown) {
							await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
								LegendaryTypes[(int)legendaryItem.Type].Items.Add(legendaryItem);
							}));
							LegendaryTypes[(int)legendaryItem.Type].recountItems();
						}
					}

					cancelToken.ThrowIfCancellationRequested();

					// get info about items replacable by legendaries
					List<Character> characters = await apiHelper.GetCharactersAsync();
					cancelToken.ThrowIfCancellationRequested();
					if (characters != null) {
						// get items that can be replaced by legendaries
						List<ItemInfo> potentials = new List<ItemInfo>();
						foreach (Character ch in characters) {
							if (ch.Level != 80) continue;
							foreach (EquipmentTab et in ch.EquipmentTabs) {
								foreach (Equipment eq in et.Equipment) {
									// check upgrade component
									bool isAquatic = eq.Slot == Equipment.SlotType.HelmAquatic || eq.Slot == Equipment.SlotType.WeaponAquaticA || eq.Slot == Equipment.SlotType.WeaponAquaticB;
									bool isArmor = eq.Slot == Equipment.SlotType.Helm || eq.Slot == Equipment.SlotType.Shoulders || eq.Slot == Equipment.SlotType.Gloves || eq.Slot == Equipment.SlotType.Coat || eq.Slot == Equipment.SlotType.Leggings || eq.Slot == Equipment.SlotType.Boots;
									bool isWeapon = eq.Slot == Equipment.SlotType.WeaponA1 || eq.Slot == Equipment.SlotType.WeaponA2 || eq.Slot == Equipment.SlotType.WeaponB1 || eq.Slot == Equipment.SlotType.WeaponB2;
									if ((isAquatic || isArmor || isWeapon) && eq.Upgrades != null) {
										foreach (int upgradeId in eq.Upgrades) {
											potentials.Add(new ItemInfo(upgradeId, ch.Name, et.Name, et.Tab, !isAquatic));
										}
									}
									// skip aquatic helm
									if (eq.Slot == Equipment.SlotType.HelmAquatic)
										continue;
									// add item to potentially wanted items
									potentials.Add(new ItemInfo(eq.Id, ch.Name, et.Name, et.Tab, !isAquatic));
								}
							}
						}
						cancelToken.ThrowIfCancellationRequested();
						if (potentials.Count > 0) {
							// get unique item ids to check for item type
							List<int> ids = potentials.Select(p => p.ItemId).Distinct().ToList();
							// get item info about items replacable by legendaries to get the proper item type
							List<Item> equippedItems = await apiHelper.GetItemsAsync(ids);
							if (equippedItems != null) {
								// count used legendary items
								foreach (Item item in equippedItems) {
									if (item.Rarity != Item.ItemRarity.Legendary)
										continue;
									// skip unknown types
									LegendaryItem.ItemType itemType = LegendaryItem.GetItemType(item);
									if (itemType == LegendaryItem.ItemType.Unknown)
										continue;
									// get all potentials with the same Id
									List<ItemInfo> potentialsWithSameId = potentials.FindAll(p => p.ItemId == item.Id);
									// add legendary items to used info
									foreach (ItemInfo p in potentialsWithSameId) {
										LegendaryTypes[(int)itemType].UsedInfo.Add(p.CharName, p.TabName, p.TabId, p.IsTerrestrial);
									}
								}
								// sort replacable items according to ItemType
								Dictionary<LegendaryItem.ItemType, List<ItemInfo>> replacableItems = new Dictionary<LegendaryItem.ItemType, List<ItemInfo>>();
								foreach (Item item in equippedItems) {
									if (item.Rarity == Item.ItemRarity.Legendary)
										continue;
									// skip unknown types
									LegendaryItem.ItemType itemType = LegendaryItem.GetItemType(item);
									if (itemType == LegendaryItem.ItemType.Unknown)
										continue;
									// get item list for the ItemType and add to it
									List<ItemInfo> itemList;
									if (!replacableItems.TryGetValue(itemType, out itemList)) {
										itemList = new List<ItemInfo>();
										replacableItems.Add(itemType, itemList);
									}
									itemList.AddRange(potentials.FindAll(p => p.ItemId == item.Id));
								}
								// count replacable items
								foreach (LegendaryItem.ItemType itemType in replacableItems.Keys) {
									List<ItemInfo> list = replacableItems[itemType];
									// sort items so that terrestrial items are first
									list.Sort((i1, i2) => { return i2.IsTerrestrial.CompareTo(i1.IsTerrestrial); });
									// split items to Usable and Needed lists
									foreach (ItemInfo ri in list) {
										// get number of legendary items of this type used in the same template
										int used = LegendaryTypes[(int)itemType].UsedInfo.GetCountFromTab(ri.CharName, ri.TabId);
										int usable = LegendaryTypes[(int)itemType].UsableInfo.GetCountFromTab(ri.CharName, ri.TabId);
										// add to usable or needed items
										if (used + usable < LegendaryTypes[(int)itemType].Count)
											LegendaryTypes[(int)itemType].UsableInfo.Add(ri.CharName, ri.TabName, ri.TabId, ri.IsTerrestrial);
										else
											LegendaryTypes[(int)itemType].NeededInfo.Add(ri.CharName, ri.TabName, ri.TabId, ri.IsTerrestrial);
									}
								}
								IsDetailLoaded = true;
							}
						}
					}
				}

			}, cancelToken);
		}

		private async void OnReloadAsync() {
			await LoadDataAsync();
		}

		private bool CanReload() {
			return !IsLoading;
		}

		private void OnSettings() {
			SettingsViewModel settingsVM = new SettingsViewModel();
			bool? result = _DialogService.ShowDialog(settingsVM);
			if (result.HasValue && result.Value) {
				// reload data when Api Key changed
				if (settingsVM.ApiKey != null && settingsVM.ApiKeyChanged) {
					LoadDataAsync();
				}
				// other settings
				NoWater = settingsVM.NoWater;
			}

		}

		private void OnInfo() {
			InfoViewModel infoVM = new InfoViewModel(_DialogService, _UpdateHelper);
			_DialogService.ShowDialog(infoVM);
		}

		private void OnTypeSelected(LegendaryType type) {
			Detail = type;
		}

	}

}
