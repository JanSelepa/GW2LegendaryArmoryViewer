using Newtonsoft.Json;
using System.Collections.Generic;

namespace gw2lav.Model {

	public class Bag {

		[JsonProperty("inventory")]
		public InventorySlot[] Inventory;

	}

}
