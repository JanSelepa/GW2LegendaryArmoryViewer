using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace gw2lav.Model {

	public class ApiHelper : IDisposable {

		private const string API_URL = "https://api.guildwars2.com";
		private const string HEADER_SCHEMA_VERSION = "2022-08-01T00:00:00Z";

		private static class ApiCommand {
			public const string LegendaryArmory = "/v2/legendaryarmory";
			public const string LegendaryArmoryCounts = "/v2/account/legendaryarmory";
			public const string Characters = "/v2/characters?ids=all";
			public const string Items = "/v2/items?ids=";
		}

		private HttpClient _HttpClient;

		public ApiHelper() {
			string apiKey = RegistryHelper.GetApiKey();

			_HttpClient = new HttpClient();
			_HttpClient.DefaultRequestHeaders.Add("X-Schema-Version", HEADER_SCHEMA_VERSION);
			if (apiKey != null)
				_HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
		}

		private async Task<string> RequestAsync(string command) {
			Uri uri = new Uri(API_URL + command);
			using (var resp = await _HttpClient.GetAsync(API_URL + command)) {
				if (resp.IsSuccessStatusCode) {
					return await resp.Content.ReadAsStringAsync();
				} else {
					throw new HttpRequestException("Http request failed: " + command);
				}
			}
		}

		public async Task<Item[]> GetItemsAsync(string ids) {
			try {
				string json = await RequestAsync(ApiCommand.Items + ids);
				Item[] items = JsonConvert.DeserializeObject<Item[]>(json);
				return items;
			} catch (Exception) {
				return null;
			}

		}

		public async Task<Item[]> GetLegendaryItemsAsync() {
			try {
				// get legendary id list
				string json = await RequestAsync(ApiCommand.LegendaryArmory);
				string[] legIds = JsonConvert.DeserializeObject<string[]>(json);
				string legIdsCombined = string.Join(",", legIds);

				// get details for each legendary id
				return await GetItemsAsync(legIdsCombined);
			} catch (Exception) {
				return null;
			}
		}

		public async Task<CountItem[]> GetLegendaryItemCountsAsync() {
			try {
				string json = await RequestAsync(ApiCommand.LegendaryArmoryCounts);
				CountItem[] legItemCounts = JsonConvert.DeserializeObject<CountItem[]>(json);
				return legItemCounts;
			} catch (Exception) {
				return null;
			}
		}

		public async Task<Character[]> GetCharactersAsync() {
			try {
				string json = await RequestAsync(ApiCommand.Characters);
				Character[] characters = JsonConvert.DeserializeObject<Character[]>(json);
				return characters;
			} catch (Exception) {
				return null;
			}
		}

		public void Dispose() {
			if (_HttpClient != null) _HttpClient.Dispose();
		}
	}

}
