using gw2lav.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace gw2lav.Service {

	interface IApiService {
		IApiHelper GetApiHelper(CancellationToken cancelToken);
	}

	interface IApiHelper : IDisposable {
		Task<List<Item>> GetItemsAsync(List<int> ids);
		Task<List<Item>> GetLegendaryItemsAsync();
		Task<List<CountItem>> GetLegendaryItemCountsAsync();
		Task<List<Character>> GetCharactersAsync();
	}

	class ApiService : IApiService {

		IRegistryService _RegistryService;

		public ApiService(IRegistryService registryService) {
			_RegistryService = registryService;
		}

		public IApiHelper GetApiHelper(CancellationToken cancelToken) {
			return new ApiHelper(_RegistryService, cancelToken);
		}

	}

	class ApiHelper : IApiHelper {

		private const string API_URL = "https://api.guildwars2.com";
		private const string HEADER_SCHEMA_VERSION = "2022-08-01T00:00:00Z";

		private const int MAX_ID_COUNT = 200;

		private static class ApiCommand {
			public const string LegendaryArmory = "/v2/legendaryarmory";
			public const string LegendaryArmoryCounts = "/v2/account/legendaryarmory";
			public const string Characters = "/v2/characters?ids=all";
			public const string Items = "/v2/items?ids=";
		}

		private HttpClient _HttpClient;
		private CancellationToken _CancelToken;

		public ApiHelper(IRegistryService registryService, CancellationToken cancelToken) {
			string apiKey = registryService.GetApiKey();

			_HttpClient = new HttpClient();
			_HttpClient.DefaultRequestHeaders.Add("X-Schema-Version", HEADER_SCHEMA_VERSION);
			if (apiKey != null)
				_HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

			_CancelToken = cancelToken;
		}

		private async Task<string> RequestAsync(string command) {
			Uri uri = new Uri(API_URL + command);
			using (var resp = await _HttpClient.GetAsync(API_URL + command, _CancelToken)) {
				if (resp.IsSuccessStatusCode) {
					return await resp.Content.ReadAsStringAsync();
				} else {
					throw new HttpRequestException("Http request failed: " + command);
				}
			}
		}

		public async Task<List<Item>> GetItemsAsync(List<int> ids) {
			try {
				// need to split the ids to more requests due to API limitations
				List<Item> items = new List<Item>();
				for (int i = 0; i <= ids.Count / MAX_ID_COUNT; i++) {
					string idsCombined = string.Join(",", ids.Skip(i * MAX_ID_COUNT).Take(MAX_ID_COUNT));
					string json = await RequestAsync(ApiCommand.Items + idsCombined);
					items.AddRange(JsonConvert.DeserializeObject<List<Item>>(json));
				}
				return items;
			} catch (Exception) {
				return null;
			}

		}

		public async Task<List<Item>> GetLegendaryItemsAsync() {
			try {
				// get legendary id list
				string json = await RequestAsync(ApiCommand.LegendaryArmory);
				List<int> legIds = JsonConvert.DeserializeObject<List<int>>(json);

				// get details for each legendary id
				return await GetItemsAsync(legIds);
			} catch (Exception) {
				return null;
			}
		}

		public async Task<List<CountItem>> GetLegendaryItemCountsAsync() {
			try {
				string json = await RequestAsync(ApiCommand.LegendaryArmoryCounts);
				List<CountItem> legItemCounts = JsonConvert.DeserializeObject<List<CountItem>>(json);
				return legItemCounts;
			} catch (Exception) {
				return null;
			}
		}

		public async Task<List<Character>> GetCharactersAsync() {
			try {
				string json = await RequestAsync(ApiCommand.Characters);
				List<Character> characters = JsonConvert.DeserializeObject<List<Character>>(json);
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
