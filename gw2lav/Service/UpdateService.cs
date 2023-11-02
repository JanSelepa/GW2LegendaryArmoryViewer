using gw2lav.Properties;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gw2lav.Service {

	interface IUpdateService {
		Task<bool> IsUpdateAvailableAsync();
		Task<string> GetAvailableVersionAsync();
		Task<string> UpdateAsync();
	}

	class UpdateService : IUpdateService {

		private const string GITHUB_LATEST_DOWNLOAD_URL = "https://github.com/JanSelepa/GW2LegendaryArmoryViewer/releases/latest/download/gw2lav.exe";
		private const string GITHUB_API_URL = "https://api.github.com/repos/JanSelepa/GW2LegendaryArmoryViewer/releases/latest";
		private const string GITHUB_USER_AGENT = "GW2LegendaryArmoryViewer";

		private const string BACKUP_PATH = "gw2lav.old";
		private const string UPDATE_PATH = "gw2lav.new";

		private IArgsService _ArgsService;

		private readonly Regex _VersionRegex = new Regex(@"v\d+.\d+.\d+");

		private Task<AppInfo> _AppInfoTask;

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

		public UpdateService(IArgsService argsService) {
			_AppInfoTask = GetAppInfoAsync();

			_ArgsService = argsService;
			if (argsService.HasArgCleanupUpdate())
				CleanupBackup();
		}

		public async Task<bool> IsUpdateAvailableAsync() {
			return (await _AppInfoTask).IsUpdateAvailable;
		}

		public async Task<string> GetAvailableVersionAsync() {
			return (await _AppInfoTask).AvailableVersion;
		}

		/*
		 * Updates this app with the latest version from Github, replaces currently running exe
		 * Returns null if successfull, otherwise returns error messsage
		 */
		public async Task<string> UpdateAsync() {
			if (!await IsUpdateAvailableAsync()) return null;

			string downloadError = await DownloadUpdateAsync();
			if (downloadError != null) return downloadError;

			string appFileName = AppDomain.CurrentDomain.FriendlyName;
			try {
				if (File.Exists(BACKUP_PATH))
					File.Delete(BACKUP_PATH);
				File.Move(appFileName, BACKUP_PATH);
				File.Move(UPDATE_PATH, appFileName);
			} catch (Exception e) {
				return string.Format(R.info_update_error_replace, e.Message);
			}

			_ArgsService.RestartAfterUpdate(appFileName);

			return null;
		}

		private async Task<AppInfo> GetAppInfoAsync() {
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

		private async Task<UpdateInfo> GetUpdateInfoAsync() {
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

		/*
		 * Downloads latest version of this app from Github
		 * Returns null if successful, otherwise returns error message
		 */
		private async Task<string> DownloadUpdateAsync() {
			try {
				WebClient webClient = new WebClient();
				await webClient.DownloadFileTaskAsync(GITHUB_LATEST_DOWNLOAD_URL, UPDATE_PATH);
				return null;
			} catch (WebException we) {
				if (we.Status == WebExceptionStatus.ProtocolError && we.Response != null) {
					HttpWebResponse wr = (HttpWebResponse)we.Response;
					return string.Format(R.info_update_error_download, "(" + wr.StatusCode + ") " + wr.StatusDescription);
				} else if (we.InnerException != null) {
					return string.Format(R.info_update_error_download, we.InnerException.Message);
				} else {
					return string.Format(R.info_update_error_download, we.Message);
				}
			} catch (Exception e) {
				return string.Format(R.info_update_error_download, e.Message);
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
