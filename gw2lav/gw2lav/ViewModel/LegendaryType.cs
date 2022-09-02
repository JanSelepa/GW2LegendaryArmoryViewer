using gw2lav.Properties;
using System.Collections.ObjectModel;
using static gw2lav.ViewModel.LegendaryItem;

namespace gw2lav.ViewModel {

	class LegendaryType : BindableBase {

		public string Name { get; set; }

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
			Name = getTypeName(type);
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

		private string getTypeName(ItemType type) {
			switch (type) {
				// Armor
				case ItemType.LightHead: case ItemType.MediumHead: case ItemType.HeavyHead:
					return R.type_armor_head;
				case ItemType.LightShoulders: case ItemType.MediumShoulders: case ItemType.HeavyShoulders:
					return R.type_armor_shoulders;
				case ItemType.LightHands: case ItemType.MediumHands: case ItemType.HeavyHands:
					return R.type_armor_hands;
				case ItemType.LightBody: case ItemType.MediumBody: case ItemType.HeavyBody:
					return R.type_armor_body;
				case ItemType.LightLegs: case ItemType.MediumLegs: case ItemType.HeavyLegs:
					return R.type_armor_legs;
				case ItemType.LightFeet: case ItemType.MediumFeet: case ItemType.HeavyFeet:
					return R.type_armor_feet;
				// Runes & Sigils
				case ItemType.Rune: return R.type_rune;
				case ItemType.Sigil: return R.type_sigil;
				// Back & Trinkets
				case ItemType.Back: return R.type_back;
				case ItemType.Accessory: return R.type_accessory;
				case ItemType.Amulet: return R.type_amulet;
				case ItemType.Ring: return R.type_ring;
				// Weapons
				case ItemType.Axe: return R.type_weapon_axe;
				case ItemType.Dagger: return R.type_weapon_dagger;
				case ItemType.Mace: return R.type_weapon_mace;
				case ItemType.Pistol: return R.type_weapon_pistol;
				case ItemType.Sword: return R.type_weapon_sword;
				case ItemType.Scepter: return R.type_weapon_scepter;
				case ItemType.Focus: return R.type_weapon_focus;
				case ItemType.Shield: return R.type_weapon_shield;
				case ItemType.Torch: return R.type_weapon_torch;
				case ItemType.Warhorn: return R.type_weapon_warhorn;
				case ItemType.Greatsword: return R.type_weapon_greatsword;
				case ItemType.Hammer: return R.type_weapon_hammer;
				case ItemType.LongBow: return R.type_weapon_longbow;
				case ItemType.Rifle: return R.type_weapon_rifle;
				case ItemType.ShortBow: return R.type_weapon_shortbow;
				case ItemType.Staff: return R.type_weapon_staff;
				case ItemType.Harpoon: return R.type_weapon_harpoon;
				case ItemType.Speargun: return R.type_weapon_speargun;
				case ItemType.Trident: return R.type_weapon_trident;
			}
			return R.type_unknown;
		}

	}

}
