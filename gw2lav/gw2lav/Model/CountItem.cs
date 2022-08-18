using Newtonsoft.Json;

namespace gw2lav.Model {

	public class CountItem {

		[JsonProperty("id")]
		public int Id;
		[JsonProperty("count")]
		public int Count;

	}

}
