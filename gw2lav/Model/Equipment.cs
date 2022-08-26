using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace gw2lav.Model {

	public class Equipment {

		[JsonConverter(typeof(StringEnumConverter))]
		public enum SlotType {
			HelmAquatic, WeaponAquaticA, WeaponAquaticB,
			Boots, Coat, Gloves, Helm, Leggings, Shoulders,
			Backpack, Accessory1, Accessory2, Amulet, Ring1, Ring2,
			WeaponA1, WeaponA2, WeaponB1, WeaponB2
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum LocationType {
			Equipped, Armory, EquippedFromLegendaryArmory, LegendaryArmory
		}

		[JsonProperty("id")]
		public int Id;

		[JsonProperty("slot")]
		public SlotType Slot;

		[JsonProperty("location")]
		public LocationType Location;

		[JsonProperty("upgrades")]
		public int[] Upgrades;

	}

}
