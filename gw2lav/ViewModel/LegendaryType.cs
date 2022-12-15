using gw2lav.Properties;
using System.Collections.ObjectModel;
using static gw2lav.ViewModel.LegendaryItem;

namespace gw2lav.ViewModel {

	class LegendaryType : BindableBase {

		public string Name { get; set; }
		public string FullName { get; set; }

		private int _Count;
		public int Count {
			get { return _Count; }
			set { SetProperty(ref _Count, value); }
		}

		public ItemType Type { get; set; }
		public bool ShowSections { get; set; }

		public ObservableCollection<LegendaryItem> Items { get; set; }

		public CountInfo UsedInfo { get; set; }
		public CountInfo UsableInfo { get; set; }
		public CountInfo NeededInfo { get; set; }

		public LegendaryType(ItemType type) {
			Type = type;
			setNames(type);
			ShowSections = type == ItemType.Sigil || type == ItemType.Rune;
			Count = 0;
			Items = new ObservableCollection<LegendaryItem>();
			UsedInfo = new CountInfo();
			UsableInfo = new CountInfo();
			NeededInfo = new CountInfo();
		}

		public void recountItems() {
			int count = 0;
			foreach (LegendaryItem item in Items) {
				count += item.Count;
			}
			Count = count;
		}

		public void AddItem(bool isLegendary, string charName, int tabId, string tabName, bool isTerrestrial) {
			// inventory items
			if (tabId == 0) {
				if (isLegendary) return;
				NeededInfo.Add(charName, tabName, tabId, isTerrestrial);
				return;
			}
			// equipment tabs items
			if (isLegendary) {
				UsedInfo.Add(charName, tabName, tabId, isTerrestrial);
			} else {
				// get number of legendary items of this type used in the same template
				int used = UsedInfo.GetCountFromTab(charName, tabId);
				int usable = UsableInfo.GetCountFromTab(charName, tabId);
				// add to usable or needed items
				if (used + usable < Count)
					UsableInfo.Add(charName, tabName, tabId, isTerrestrial);
				else
					NeededInfo.Add(charName, tabName, tabId, isTerrestrial);
			}
		}

		private void setNames(ItemType type) {
			switch (type) {
				// Armor
				case ItemType.LightHead: Name = R.type_armor_head; FullName = R.type_armor_head_light; break;
				case ItemType.MediumHead: Name = R.type_armor_head; FullName = R.type_armor_head_medium; break;
				case ItemType.HeavyHead: Name = R.type_armor_head; FullName = R.type_armor_head_heavy; break;
				case ItemType.LightShoulders: Name = R.type_armor_shoulders; FullName = R.type_armor_shoulders_light; break;
				case ItemType.MediumShoulders: Name = R.type_armor_shoulders; FullName = R.type_armor_shoulders_medium; break;
				case ItemType.HeavyShoulders: Name = R.type_armor_shoulders; FullName = R.type_armor_shoulders_heavy; break;
				case ItemType.LightHands: Name = R.type_armor_hands; FullName = R.type_armor_hands_light; break;
				case ItemType.MediumHands: Name = R.type_armor_hands; FullName = R.type_armor_hands_medium; break;
				case ItemType.HeavyHands: Name = R.type_armor_hands; FullName = R.type_armor_hands_heavy; break;
				case ItemType.LightBody: Name = R.type_armor_body; FullName = R.type_armor_body_light; break;
				case ItemType.MediumBody: Name = R.type_armor_body; FullName = R.type_armor_body_medium; break;
				case ItemType.HeavyBody: Name = R.type_armor_body; FullName = R.type_armor_body_heavy; break;
				case ItemType.LightLegs: Name = R.type_armor_legs; FullName = R.type_armor_legs_light; break;
				case ItemType.MediumLegs: Name = R.type_armor_legs; FullName = R.type_armor_legs_medium; break;
				case ItemType.HeavyLegs: Name = R.type_armor_legs; FullName = R.type_armor_legs_heavy; break;
				case ItemType.LightFeet: Name = R.type_armor_feet; FullName = R.type_armor_feet_light; break;
				case ItemType.MediumFeet: Name = R.type_armor_feet; FullName = R.type_armor_feet_medium; break;
				case ItemType.HeavyFeet: Name = R.type_armor_feet; FullName = R.type_armor_feet_heavy; break;
				// Runes & Sigils
				case ItemType.Rune: Name = FullName = R.type_rune; break;
				case ItemType.Sigil: Name = FullName = R.type_sigil; break;
				// Back & Trinkets
				case ItemType.Back: Name = FullName = R.type_back; break;
				case ItemType.Accessory: Name = FullName = R.type_accessory; break;
				case ItemType.Amulet: Name = FullName = R.type_amulet; break;
				case ItemType.Ring: Name = FullName = R.type_ring; break;
				// Weapons
				case ItemType.Axe: Name = FullName = R.type_weapon_axe; break;
				case ItemType.Dagger: Name = FullName = R.type_weapon_dagger; break;
				case ItemType.Mace: Name = FullName = R.type_weapon_mace; break;
				case ItemType.Pistol: Name = FullName = R.type_weapon_pistol; break;
				case ItemType.Sword: Name = FullName = R.type_weapon_sword; break;
				case ItemType.Scepter: Name = FullName = R.type_weapon_scepter; break;
				case ItemType.Focus: Name = FullName = R.type_weapon_focus; break;
				case ItemType.Shield: Name = FullName = R.type_weapon_shield; break;
				case ItemType.Torch: Name = FullName = R.type_weapon_torch; break;
				case ItemType.Warhorn: Name = FullName = R.type_weapon_warhorn; break;
				case ItemType.Greatsword: Name = FullName = R.type_weapon_greatsword; break;
				case ItemType.Hammer: Name = FullName = R.type_weapon_hammer; break;
				case ItemType.LongBow: Name = FullName = R.type_weapon_longbow; break;
				case ItemType.Rifle: Name = FullName = R.type_weapon_rifle; break;
				case ItemType.ShortBow: Name = FullName = R.type_weapon_shortbow; break;
				case ItemType.Staff: Name = FullName = R.type_weapon_staff; break;
				case ItemType.Harpoon: Name = FullName = R.type_weapon_harpoon; break;
				case ItemType.Speargun: Name = FullName = R.type_weapon_speargun; break;
				case ItemType.Trident: Name = FullName = R.type_weapon_trident; break;
				// Unknown
				default: Name = FullName = R.type_unknown; break;
			}
		}

	}

}
