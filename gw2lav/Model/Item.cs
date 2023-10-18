using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace gw2lav.Model {

	public class Item {

		[JsonConverter(typeof(TolerantStringEnumConverter))]
		public enum ItemType {
			Unknown,
			Armor,
			Back,
			Bag,
			Consumable,
			Container,
			CraftingMaterial,
			Gathering,
			Gizmo,
			Key,
			MiniPet,
			Tool,
			Trait,
			Trinket,
			Trophy,
			UpgradeComponent,
			Weapon
		}

		[JsonConverter(typeof(TolerantStringEnumConverter))]
		public enum ItemSubType {
			Unknown,
			HelmAquatic,
			Boots, Coat, Gloves, Helm, Leggings, Shoulders,
			Accessory, Amulet, Ring,
			Default, Gem, Rune, Sigil,
			Axe, Dagger, Mace, Pistol, Scepter, Sword, Focus, Shield, Torch, Warhorn,
			Greatsword, Hammer, LongBow, Rifle, ShortBow, Staff, Harpoon, Speargun, Trident
		}

		[JsonConverter(typeof(TolerantStringEnumConverter))]
		public enum ItemWeightClass {
			Unknown,
			Light, Medium, Heavy
		}

		[JsonConverter(typeof(TolerantStringEnumConverter))]
		public enum ItemRarity {
			Unknown,
			Junk, Basic, Fine, Masterwork, Rare, Exotic, Ascended, Legendary
		}

		public class ItemDetails {

			[JsonProperty("type")]
			public ItemSubType Type;
			[JsonProperty("weight_class")]
			public ItemWeightClass WeightClass;

		}



		[JsonProperty("id")]
		public int Id;

		[JsonProperty("name")]
		public string Name;

		[JsonProperty("type")]
		public ItemType Type;

		[JsonProperty("icon")]
		public string Icon;

		[JsonProperty("rarity")]
		public ItemRarity Rarity;

		[JsonProperty("details")]
		public ItemDetails Details;

	}

}
