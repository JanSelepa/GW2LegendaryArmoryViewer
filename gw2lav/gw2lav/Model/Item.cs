using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace gw2lav.Model {

	public class Item {

		[JsonConverter(typeof(StringEnumConverter))]
		public enum ItemType {
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

		[JsonConverter(typeof(StringEnumConverter))]
		public enum ItemSubType {
			Boots, Coat, Gloves, Helm, Leggings, Shoulders,
			Accessory, Amulet, Ring,
			Rune, Sigil,
			Axe, Dagger, Mace, Pistol, Scepter, Sword, Focus, Shield, Torch, Warhorn,
			Greatsword, Hammer, LongBow, Rifle, ShortBow, Staff, Harpoon, Speargun, Trident
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum ItemWeightClass {
			Light, Medium, Heavy
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

		[JsonProperty("details")]
		public ItemDetails Details;

	}

}
