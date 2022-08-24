using Newtonsoft.Json;

namespace gw2lav.Model {

	public class EquipmentTab {

		[JsonProperty("tab")]
		public int Tab;

		[JsonProperty("name")]
		public string Name;

		[JsonProperty("equipment")]
		public Equipment[] Equipment;

	}

}
