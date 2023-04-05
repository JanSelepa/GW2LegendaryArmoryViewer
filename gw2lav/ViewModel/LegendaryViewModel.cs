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
			public LegendaryItem.ItemType ItemType;
			public Item.ItemRarity ItemRarity;
			public int[] ItemUpgrades;
			public ItemInfo(int itemId, string charName, int tabId, string tabName, bool isTerrestrial) {
				ItemId = itemId;
				CharName = charName;
				TabName = tabName;
				TabId = tabId;
				IsTerrestrial = isTerrestrial;
			}
			public ItemInfo(int itemId, int[] itemUpgrades, string charName, int tabId, string tabName) {
				ItemId = itemId;
				CharName = charName;
				TabName = tabName;
				TabId = tabId;
				ItemUpgrades = itemUpgrades;
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

		private bool _NoInventory;
		public bool NoInventory {
			get { return _NoInventory; }
			set { SetProperty(ref _NoInventory, value); }
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
			NoInventory = RegistryHelper.GetNoInventory();
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

						List<ItemInfo> itemsAll = new List<ItemInfo>();

						// make a list of all items on a character (no upgrades yet)
						foreach (Character chr in characters) {
							// ignore non max level characters
							if (chr.Level != 80) continue;

							// look through equipment templates
							foreach (EquipmentTab tab in chr.EquipmentTabs) {
								foreach (Equipment eqp in tab.Equipment) {
									itemsAll.Add(new ItemInfo(eqp.Id, eqp.Upgrades, chr.Name, tab.Id, tab.Name));
								}
							}

							// look through inventory
							foreach (Bag bag in chr.Bags) {
								foreach (InventorySlot isl in bag.Inventory) {
									// ignore empty inventory slots and unbound items
									if (isl == null || isl.Binding == null) continue;
									for (int i = 0; i < isl.Count; i++)
										itemsAll.Add(new ItemInfo(isl.Id, isl.Upgrades, chr.Name, 0, null));
								}
							}

						}

						cancelToken.ThrowIfCancellationRequested();

						// get item info about items
						List<int> idsAllUnique = itemsAll.Select(p => p.ItemId).Distinct().ToList();
						List<Item> itemsAllUnique = await apiHelper.GetItemsAsync(idsAllUnique);
						if (itemsAllUnique != null) {
							foreach (Item item in itemsAllUnique) {
								List<ItemInfo> itemsSameId = itemsAll.FindAll(p => p.ItemId == item.Id);
								LegendaryItem.ItemType itemType = LegendaryItem.GetItemType(item);
								bool isTerrestrial = LegendaryItem.GetIsTerrestrial(item);
								foreach (ItemInfo ii in itemsSameId) {
									ii.ItemRarity = item.Rarity;
									ii.ItemType = itemType;
									ii.IsTerrestrial = isTerrestrial;
								}
							}
						}

						cancelToken.ThrowIfCancellationRequested();

						// get a complete list of upgrades
						List<ItemInfo> itemsUpgradesAll = new List<ItemInfo>();
						foreach (ItemInfo itemInfo in itemsAll) {
							if (itemInfo.ItemUpgrades == null) continue;
							foreach (int upgrade in itemInfo.ItemUpgrades) {
								itemsUpgradesAll.Add(new ItemInfo(upgrade, itemInfo.CharName, itemInfo.TabId, itemInfo.TabName, itemInfo.IsTerrestrial));
							}
						}

						cancelToken.ThrowIfCancellationRequested();

						// get info about upgrades
						List<int> idsUpgradesUnique = itemsUpgradesAll.Select(p => p.ItemId).Distinct().ToList();
						List<Item> itemsUpgradesUnique = await apiHelper.GetItemsAsync(idsUpgradesUnique);
						if (itemsUpgradesUnique != null) {
							foreach (Item item in itemsUpgradesUnique) {
								List<ItemInfo> itemsSameId = itemsUpgradesAll.FindAll(p => p.ItemId == item.Id);
								LegendaryItem.ItemType itemType = LegendaryItem.GetItemType(item);
								foreach (ItemInfo ii in itemsSameId) {
									ii.ItemRarity = item.Rarity;
									ii.ItemType = itemType;
								}
							}
						}

						cancelToken.ThrowIfCancellationRequested();

						// add upgrades to the rest of the items
						itemsAll.AddRange(itemsUpgradesAll);

						// sort list for proper counting
						itemsAll.Sort((i1, i2) => {
							if (i1.ItemRarity != i2.ItemRarity) {
								if (i1.ItemRarity == Item.ItemRarity.Legendary) return -1;
								if (i2.ItemRarity == Item.ItemRarity.Legendary) return 1;
							}
							return i2.IsTerrestrial.CompareTo(i1.IsTerrestrial);
						});

						// count items
						foreach (ItemInfo itemInfo in itemsAll) {
							// skip unknown types
							if (itemInfo.ItemType == LegendaryItem.ItemType.Unknown)
								continue;
							LegendaryTypes[(int)itemInfo.ItemType].AddItem(itemInfo.ItemRarity == Item.ItemRarity.Legendary, itemInfo.CharName, itemInfo.TabId, itemInfo.TabName, itemInfo.IsTerrestrial);
						}

						IsDetailLoaded = true;
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
				NoInventory = settingsVM.NoInventory;
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
