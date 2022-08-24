using Newtonsoft.Json;

namespace gw2lav.Model {

	public class Character {

		[JsonProperty("name")]
		public string Name;

		[JsonProperty("level")]
		public int Level;

		[JsonProperty("equipment_tabs")]
		public EquipmentTab[] EquipmentTabs;

	}

}
