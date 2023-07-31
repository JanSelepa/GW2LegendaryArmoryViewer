using Microsoft.Win32;
using System;

namespace gw2lav {

	public class RegistryHelper {

		private const string KEY_API_KEY = "api_key";
		private const string KEY_NO_WATER = "no_water";
		private const string KEY_NO_INVENTORY = "no_inventory";
		private const string KEY_EXPAND_EQUIPNEEDED = "expand_equipneeded";
		private const string KEY_EXPAND_EQUIPUSABLE = "expand_equipusable";
		private const string KEY_EXPAND_EQUIPUSED = "expand_equipused";
		private const string KEY_EXPAND_INVENTORY = "expand_inventory";

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

		private static bool GetBoolValue(string keyName, bool defaultValue) {
			int? keyValue = (int?)GetValue(keyName);
			if (keyValue.HasValue)
				return keyValue.Value != 0;
			else
				return defaultValue;
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
			return GetBoolValue(KEY_NO_WATER, true);
		}

		public static bool SetNoWater(bool noWater) {
			return SetValue(KEY_NO_WATER, noWater ? 1 : 0);
		}

		// No Inventory

		public static bool GetNoInventory() {
			return GetBoolValue(KEY_NO_INVENTORY, false);
		}

		public static bool SetNoInventory(bool noInventory) {
			return SetValue(KEY_NO_INVENTORY, noInventory ? 1 : 0);
		}

		// Expand statuses for Detail View

		public static bool GetExpandEquipNeeded() {
			return GetBoolValue(KEY_EXPAND_EQUIPNEEDED, true);
		}

		public static bool SetExpandEquipNeeded(bool value) {
			return SetValue(KEY_EXPAND_EQUIPNEEDED, value ? 1 : 0);
		}

		public static bool GetExpandEquipUsable() {
			return GetBoolValue(KEY_EXPAND_EQUIPUSABLE, true);
		}

		public static bool SetExpandEquipUsable(bool value) {
			return SetValue(KEY_EXPAND_EQUIPUSABLE, value ? 1 : 0);
		}

		public static bool GetExpandEquipUsed() {
			return GetBoolValue(KEY_EXPAND_EQUIPUSED, false);
		}

		public static bool SetExpandEquipUsed(bool value) {
			return SetValue(KEY_EXPAND_EQUIPUSED, value ? 1 : 0);
		}

		public static bool GetExpandInventory() {
			return GetBoolValue(KEY_EXPAND_INVENTORY, true);
		}

		public static bool SetExpandInventory(bool value) {
			return SetValue(KEY_EXPAND_INVENTORY, value ? 1 : 0);
		}

	}

}
