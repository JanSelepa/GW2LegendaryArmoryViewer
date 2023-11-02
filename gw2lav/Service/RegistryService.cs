using Microsoft.Win32;
using System;

namespace gw2lav.Service {

	interface IRegistryService {
		string GetApiKey();
		bool SetApiKey(string apiKey);
		bool GetNoWater();
		bool SetNoWater(bool noWater);
		bool GetNoInventory();
		bool SetNoInventory(bool noInventory);
		bool GetExpandEquipNeeded();
		bool SetExpandEquipNeeded(bool value);
		bool GetExpandEquipUsable();
		bool SetExpandEquipUsable(bool value);
		bool GetExpandEquipUsed();
		bool SetExpandEquipUsed(bool value);
		bool GetExpandInventory();
		bool SetExpandInventory(bool value);
	}

	class RegistryService : IRegistryService {

		private const string KEY_API_KEY = "api_key";
		private const string KEY_NO_WATER = "no_water";
		private const string KEY_NO_INVENTORY = "no_inventory";
		private const string KEY_EXPAND_EQUIPNEEDED = "expand_equipneeded";
		private const string KEY_EXPAND_EQUIPUSABLE = "expand_equipusable";
		private const string KEY_EXPAND_EQUIPUSED = "expand_equipused";
		private const string KEY_EXPAND_INVENTORY = "expand_inventory";

		private const string REG_PATH = "Software\\GW2LegendaryArmoryViewer\\";

		private static RegistryKey _HKCU = Registry.CurrentUser;

		private object GetValue(string keyName) {
			try {
				RegistryKey rk = _HKCU.OpenSubKey(REG_PATH);
				if (rk == null) return null;
				return rk.GetValue(keyName);
			} catch (Exception) {
				return null;
			}
		}

		private bool SetValue(string keyName, object keyValue) {
			try {
				RegistryKey rk = _HKCU.CreateSubKey(REG_PATH);
				if (rk == null) return false;
				rk.SetValue(keyName, keyValue);
				return true;
			} catch (Exception) {
				return false;
			}
		}

		private bool GetBoolValue(string keyName, bool defaultValue) {
			int? keyValue = (int?)GetValue(keyName);
			if (keyValue.HasValue)
				return keyValue.Value != 0;
			else
				return defaultValue;
		}

		// Api Key

		public string GetApiKey() {
			return (string)GetValue(KEY_API_KEY);
		}

		public bool SetApiKey(string apiKey) {
			return SetValue(KEY_API_KEY, apiKey);
		}

		// No Water

		public bool GetNoWater() {
			return GetBoolValue(KEY_NO_WATER, true);
		}

		public bool SetNoWater(bool noWater) {
			return SetValue(KEY_NO_WATER, noWater ? 1 : 0);
		}

		// No Inventory

		public bool GetNoInventory() {
			return GetBoolValue(KEY_NO_INVENTORY, false);
		}

		public bool SetNoInventory(bool noInventory) {
			return SetValue(KEY_NO_INVENTORY, noInventory ? 1 : 0);
		}

		// Expand statuses for Detail View

		public bool GetExpandEquipNeeded() {
			return GetBoolValue(KEY_EXPAND_EQUIPNEEDED, true);
		}

		public bool SetExpandEquipNeeded(bool value) {
			return SetValue(KEY_EXPAND_EQUIPNEEDED, value ? 1 : 0);
		}

		public bool GetExpandEquipUsable() {
			return GetBoolValue(KEY_EXPAND_EQUIPUSABLE, true);
		}

		public bool SetExpandEquipUsable(bool value) {
			return SetValue(KEY_EXPAND_EQUIPUSABLE, value ? 1 : 0);
		}

		public bool GetExpandEquipUsed() {
			return GetBoolValue(KEY_EXPAND_EQUIPUSED, false);
		}

		public bool SetExpandEquipUsed(bool value) {
			return SetValue(KEY_EXPAND_EQUIPUSED, value ? 1 : 0);
		}

		public bool GetExpandInventory() {
			return GetBoolValue(KEY_EXPAND_INVENTORY, true);
		}

		public bool SetExpandInventory(bool value) {
			return SetValue(KEY_EXPAND_INVENTORY, value ? 1 : 0);
		}

	}

}
