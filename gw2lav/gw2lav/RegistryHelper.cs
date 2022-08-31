using Microsoft.Win32;
using System;

namespace gw2lav {

	public class RegistryHelper {

		private const string KEY_API_KEY = "api_key";
		private const string KEY_NO_WATER = "no_water";

		private const string REG_PATH = "Software\\GW2LegendaryArmoryViewer\\";

		private static RegistryKey _HKCU = Registry.CurrentUser;

		private static object GetValue(string keyName) {
			try {
				RegistryKey rk = _HKCU.OpenSubKey(REG_PATH);
				if (rk == null) return null;
				return rk.GetValue(keyName);
			} catch (Exception) {
				return null;
			}
		}

		private static bool SetValue(string keyName, object keyValue) {
			try {
				RegistryKey rk = _HKCU.CreateSubKey(REG_PATH);
				if (rk == null) return false;
				rk.SetValue(keyName, keyValue);
				return true;
			} catch (Exception) {
				return false;
			}
		}

		// Api Key

		public static string GetApiKey() {
			return (string)GetValue(KEY_API_KEY);
		}

		public static bool SetApiKey(string apiKey) {
			return SetValue(KEY_API_KEY, apiKey);
		}

		// No Water

		public static bool GetNoWater() {
			int? noWater = (int?)GetValue(KEY_NO_WATER);
			if (noWater.HasValue)
				return noWater.Value != 0;
			else
				return false;
		}

		public static bool SetNoWater(bool noWater) {
			return SetValue(KEY_NO_WATER, noWater ? 1 : 0);
		}

	}

}
