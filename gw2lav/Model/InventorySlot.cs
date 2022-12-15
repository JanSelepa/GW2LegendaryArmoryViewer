using Newtonsoft.Json;

namespace gw2lav.Model {

	public class InventorySlot {

		[JsonProperty("id")]
		public int Id;

		[JsonProperty("count")]
		public int Count;

		[JsonProperty("upgrades")]
		public int[] Upgrades;

	}

}
