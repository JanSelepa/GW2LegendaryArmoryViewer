using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gw2lav {

	interface IUpdateHelper {
		Task<bool> IsUpdateAvailableAsync();
		Task<string> GetAvailableVersionAsync();
		Task<bool> UpdateAsync();
	}

	class UpdateHelper : IUpdateHelper {

		private const string GITHUB_LATEST_DOWNLOAD_URL = "https://github.com/JanSelepa/GW2LegendaryArmoryViewer/releases/latest/download/gw2lav.exe";
		private const string GITHUB_API_URL = "https://api.github.com/repos/JanSelepa/GW2LegendaryArmoryViewer/releases/latest";
		private const string GITHUB_USER_AGENT = "GW2LegendaryArmoryViewer";

		private const string BACKUP_PATH = "gw2lav.old";
		private const string UPDATE_PATH = "gw2lav.new";

		private IArgsHelper _ArgsHelper;

		private static Regex _VersionRegex = new Regex(@"v\d+.\d+.\d+");

		private static Task<AppInfo> _AppInfoTask = GetAppInfoAsync();

		private class UpdateInfo {
			[JsonProperty("tag_name")]
			public string TagName;
		}

		private class AppInfo {
			public bool IsUpdateAvailable;
			public string AvailableVersion;
			public AppInfo(bool isUpdateAvailable = false, string availableVersion = null) {
				IsUpdateAvailable = isUpdateAvailable;
				AvailableVersion = availableVersion;
			}
		}

		public UpdateHelper(IArgsHelper argsHelper) {
			_ArgsHelper = argsHelper;
			if (argsHelper.HasArgCleanupUpdate())
				CleanupBackup();
		}

		public async Task<bool> IsUpdateAvailableAsync() {
			return (await _AppInfoTask).IsUpdateAvailable;
		}

		public async Task<string> GetAvailableVersionAsync() {
			return (await _AppInfoTask).AvailableVersion;
		}

		public async Task<bool> UpdateAsync() {
			if (!await IsUpdateAvailableAsync()) return true;

			if (!await DownloadUpdateAsync()) return false;

			string appFileName = AppDomain.CurrentDomain.FriendlyName;
			try {
				if (File.Exists(BACKUP_PATH))
					File.Delete(BACKUP_PATH);
				File.Move(appFileName, BACKUP_PATH);
				File.Move(UPDATE_PATH, appFileName);
			} catch (Exception) {
				return false;
			}

			_ArgsHelper.RestartAfterUpdate(appFileName);

			return true;
		}

		private static async Task<AppInfo> GetAppInfoAsync() {
			UpdateInfo updateInfo = await GetUpdateInfoAsync();
			if (updateInfo == null)
				return new AppInfo(false, null);

			Match versionMatch = _VersionRegex.Match(updateInfo.TagName);
			string remoteVersion = versionMatch.Value;
			if (remoteVersion == null || remoteVersion.Length == 0)
				return new AppInfo(false, null);

			try {
				remoteVersion = remoteVersion.Substring(1);     // remove the leading "v"
				Version currVersion = Assembly.GetExecutingAssembly().GetName().Version;
				string[] availableVersion = remoteVersion.Split('.');
				int verMajor = int.Parse(availableVersion[0]);
				int verMinor = int.Parse(availableVersion[1]);
				int verBuild = int.Parse(availableVersion[2]);
				bool isUpdateAvailable = verMajor > currVersion.Major || (verMajor == currVersion.Major && verMinor > currVersion.Minor) || (verMajor == currVersion.Major && verMinor == currVersion.Minor && verBuild > currVersion.Build);
				return new AppInfo(isUpdateAvailable, remoteVersion);
			} catch (Exception) {
				return new AppInfo(false, null);
			}
		}

		private static async Task<UpdateInfo> GetUpdateInfoAsync() {
			using (HttpClient httpClient = new HttpClient()) {
				httpClient.DefaultRequestHeaders.Add("User-Agent", GITHUB_USER_AGENT);
				using (var resp = await httpClient.GetAsync(GITHUB_API_URL)) {
					if (resp.IsSuccessStatusCode) {
						string responseJson = await resp.Content.ReadAsStringAsync();
						return JsonConvert.DeserializeObject<UpdateInfo>(responseJson);
					} else {
						throw new HttpRequestException("Update info request failed!");
					}
				}
			}
		}

		private async Task<bool> DownloadUpdateAsync() {
			try {
				WebClient webClient = new WebClient();
				await webClient.DownloadFileTaskAsync(GITHUB_LATEST_DOWNLOAD_URL, UPDATE_PATH);
				return true;
			} catch (Exception) {
				return false;
			}
		}

		private void CleanupBackup() {
			try {
				if (File.Exists(BACKUP_PATH))
					File.Delete(BACKUP_PATH);
			} catch (Exception) {}
		}

	}

}
