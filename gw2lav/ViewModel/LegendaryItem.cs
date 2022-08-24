using gw2lav.Model;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace gw2lav.ViewModel {

	class LegendaryItem : BindableBase {

		private static readonly string CACHE_DIR = "GW2LegendaryArmoryViewer";
		private static readonly string CACHE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CACHE_DIR, "Images");

		public enum ItemType {
			Unknown = -1,
			LightHead, LightShoulders, LightHands, LightBody, LightLegs, LightFeet,
			MediumHead, MediumShoulders, MediumHands, MediumBody, MediumLegs, MediumFeet,
			HeavyHead, HeavyShoulders, HeavyHands, HeavyBody, HeavyLegs, HeavyFeet,
			Rune, Sigil,
			Back, Accessory, Amulet, Ring,
			Axe, Dagger, Mace, Pistol, Sword,
			Scepter,
			Focus, Shield, Torch, Warhorn,
			Greatsword, Hammer, LongBow, Rifle, ShortBow, Staff,
			Harpoon, Speargun, Trident
		}

		public ItemType Type;

		public string Name { get; set; }

		private string _IconUrl;
		public string IconUrl {
			get { return _IconUrl; }
			set {
				if (SetProperty(ref _IconUrl, value))
					GetCachedImageAsync();
			}
		}

		private string _LocalIcon;
		public string LocalIcon {
			get { return _LocalIcon; }
			set { SetProperty(ref _LocalIcon, value); }
		}

		private int _Count;
		public int Count {
			get { return _Count; }
			set { SetProperty(ref _Count, value); }
		}

		public LegendaryItem(Item item, int count) {
			Name = item.Name;
			IconUrl = item.Icon;
			Count = count;
			Type = GetItemType(item);
		}

		private async Task GetCachedImageAsync() {
			Uri uri = new Uri(IconUrl);
			string localFile = string.Format(Path.Combine(CACHE_PATH, uri.Segments[uri.Segments.Length - 1]));

			if (!File.Exists(localFile)) {

				if (!Directory.Exists(CACHE_PATH))
					Directory.CreateDirectory(CACHE_PATH);

				using (WebClient webClient = new WebClient()) {
					Task downloadTask = webClient.DownloadFileTaskAsync(uri, localFile);
					await downloadTask;
					if (!downloadTask.IsCompletedSuccessfully) {
						if (File.Exists(localFile))
							File.Delete(localFile);
						return;
					}
				}

			}

			LocalIcon = localFile;
		}

		public static ItemType GetItemType(Item item) {
			ItemType itemType = ItemType.Unknown;
			switch (item.Type) {
				// Armor
				case Item.ItemType.Armor:
					switch (item.Details.WeightClass) {
						case Item.ItemWeightClass.Light:
							switch (item.Details.Type) {
								case Item.ItemSubType.Helm: itemType = ItemType.LightHead; break;
								case Item.ItemSubType.Shoulders: itemType = ItemType.LightShoulders; break;
								case Item.ItemSubType.Gloves: itemType = ItemType.LightHands; break;
								case Item.ItemSubType.Coat: itemType = ItemType.LightBody; break;
								case Item.ItemSubType.Leggings: itemType = ItemType.LightLegs; break;
								case Item.ItemSubType.Boots: itemType = ItemType.LightFeet; break;
							}
							break;
						case Item.ItemWeightClass.Medium:
							switch (item.Details.Type) {
								case Item.ItemSubType.Helm: itemType = ItemType.MediumHead; break;
								case Item.ItemSubType.Shoulders: itemType = ItemType.MediumShoulders; break;
								case Item.ItemSubType.Gloves: itemType = ItemType.MediumHands; break;
								case Item.ItemSubType.Coat: itemType = ItemType.MediumBody; break;
								case Item.ItemSubType.Leggings: itemType = ItemType.MediumLegs; break;
								case Item.ItemSubType.Boots: itemType = ItemType.MediumFeet; break;
							}
							break;
						case Item.ItemWeightClass.Heavy:
							switch (item.Details.Type) {
								case Item.ItemSubType.Helm: itemType = ItemType.HeavyHead; break;
								case Item.ItemSubType.Shoulders: itemType = ItemType.HeavyShoulders; break;
								case Item.ItemSubType.Gloves: itemType = ItemType.HeavyHands; break;
								case Item.ItemSubType.Coat: itemType = ItemType.HeavyBody; break;
								case Item.ItemSubType.Leggings: itemType = ItemType.HeavyLegs; break;
								case Item.ItemSubType.Boots: itemType = ItemType.HeavyFeet; break;
							}
							break;
					}
					break;
				// Back item
				case Item.ItemType.Back:
					itemType = ItemType.Back;
					break;
				// Trinkets
				case Item.ItemType.Trinket:
					switch (item.Details.Type) {
						case Item.ItemSubType.Amulet: itemType = ItemType.Amulet; break;
						case Item.ItemSubType.Accessory: itemType = ItemType.Accessory; break;
						case Item.ItemSubType.Ring: itemType = ItemType.Ring; break;
					}
					break;
				// Runes & Sigils
				case Item.ItemType.UpgradeComponent:
					switch (item.Details.Type) {
						case Item.ItemSubType.Rune: itemType = ItemType.Rune; break;
						case Item.ItemSubType.Sigil: itemType = ItemType.Sigil; break;
					}
					break;
				// Weapons
				case Item.ItemType.Weapon:
					switch (item.Details.Type) {
						case Item.ItemSubType.Axe: itemType = ItemType.Axe; break;
						case Item.ItemSubType.Dagger: itemType = ItemType.Dagger; break;
						case Item.ItemSubType.Mace: itemType = ItemType.Mace; break;
						case Item.ItemSubType.Pistol: itemType = ItemType.Pistol; break;
						case Item.ItemSubType.Sword: itemType = ItemType.Sword; break;
						case Item.ItemSubType.Scepter: itemType = ItemType.Scepter; break;
						case Item.ItemSubType.Focus: itemType = ItemType.Focus; break;
						case Item.ItemSubType.Shield: itemType = ItemType.Shield; break;
						case Item.ItemSubType.Torch: itemType = ItemType.Torch; break;
						case Item.ItemSubType.Warhorn: itemType = ItemType.Warhorn; break;
						case Item.ItemSubType.Greatsword: itemType = ItemType.Greatsword; break;
						case Item.ItemSubType.Hammer: itemType = ItemType.Hammer; break;
						case Item.ItemSubType.LongBow: itemType = ItemType.LongBow; break;
						case Item.ItemSubType.Rifle: itemType = ItemType.Rifle; break;
						case Item.ItemSubType.ShortBow: itemType = ItemType.ShortBow; break;
						case Item.ItemSubType.Staff: itemType = ItemType.Staff; break;
						case Item.ItemSubType.Harpoon: itemType = ItemType.Harpoon; break;
						case Item.ItemSubType.Speargun: itemType = ItemType.Speargun; break;
						case Item.ItemSubType.Trident: itemType = ItemType.Trident; break;
					}
					break;
			}
			return itemType;
		}

	}

}
