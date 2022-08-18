using Microsoft.Win32;
using System;

namespace gw2lav {

	public class RegistryHelper {

		private const string KEY_API_KEY = "api_key";

		private const string REG_PATH = "Software\\GW2LegendaryArmoryViewer\\";

		private static RegistryKey mHKCU = Registry.CurrentUser;

		private static object getValue(string keyName) {
			try {
				RegistryKey rk = mHKCU.OpenSubKey(REG_PATH);
				if (rk == null) return null;
				return rk.GetValue(keyName);
			} catch (Exception) {
				return null;
			}
		}

		private static bool setValue(string keyName, object keyValue) {
			try {
				RegistryKey rk = mHKCU.CreateSubKey(REG_PATH);
				if (rk == null) return false;
				rk.SetValue(keyName, keyValue);
				return true;
			} catch (Exception) {
				return false;
			}
		}

		public static string getApiKey() {
			return (string)getValue(KEY_API_KEY);
		}

		public static bool setApiKey(string apiKey) {
			return setValue(KEY_API_KEY, apiKey);
		}

	}

}
