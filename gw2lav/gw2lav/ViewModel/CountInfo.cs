using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace gw2lav.ViewModel {

	class CountInfo : BindableBase {

		public class Tab : BindableBase {
			public string Name { get; set; }
			public int Id { get; set; }
			public int Count { get; set; }
			public Tab(string name, int id) {
				Name = name;
				Id = id;
				Count = 1;
			}
		}

		public class Character : BindableBase {
			public string Name { get; set; }
			public int Count { get; set; }
			public ObservableCollection<Tab> Tabs { get; set; }
			public Character(string name) {
				Name = name;
				Count = 0;
				Tabs = new ObservableCollection<Tab>();
			}
		}

		public ObservableCollection<Character> TerrestrialCharacters { get; set; }
		public ObservableCollection<Character> AquaticCharacters { get; set; }

		private int _TotalCount;
		public int TotalCount {
			get { return _TotalCount; }
			set { SetProperty(ref _TotalCount, value); }
		}

		private int _TerrestrialCount;
		public int TerrestrialCount {
			get { return _TerrestrialCount; }
			set { SetProperty(ref _TerrestrialCount, value); }
		}

		private int _AquaticCount;
		public int AquaticCount {
			get { return _AquaticCount; }
			set { SetProperty(ref _AquaticCount, value); }
		}

		public CountInfo() {
			TerrestrialCharacters = new ObservableCollection<Character>();
			AquaticCharacters = new ObservableCollection<Character>();
			TerrestrialCount = 0;
			AquaticCount = 0;
			TotalCount = 0;
		}

		public void Add(string characterName, string tabName, int tabId, bool isTerrestrial) {
			bool charFound = false;
			foreach (Character ch in isTerrestrial ? TerrestrialCharacters : AquaticCharacters) {
				if (characterName == ch.Name) {
					charFound = true;
					ch.Count++;
					bool tabFound = false;
					foreach (Tab tab in ch.Tabs) {
						if (tabName == tab.Name) {
							tabFound = true;
							tab.Count++;
							break;
						}
					}
					if (!tabFound) {
						Application.Current.Dispatcher.Invoke(new Action(() => ch.Tabs.Add(new Tab(tabName, tabId))));
					}
					break;
				}
			}
			if (!charFound) {
				Character newChar = new Character(characterName);
				newChar.Count++;
				newChar.Tabs.Add(new Tab(tabName, tabId));
				Application.Current.Dispatcher.Invoke(new Action(() => {
					if (isTerrestrial)
						TerrestrialCharacters.Add(newChar);
					else
						AquaticCharacters.Add(newChar);
				}));
			}
			if (isTerrestrial)
				TerrestrialCount++;
			else
				AquaticCount++;
			TotalCount++;
		}

		public int GetCountFromTab(string charName, int tabId, bool isTerrestrial) {
			Character character = isTerrestrial ? TerrestrialCharacters.SingleOrDefault(c => c.Name == charName) : AquaticCharacters.SingleOrDefault(c => c.Name == charName);
			if (character == null)
				return 0;

			Tab tab = character.Tabs.SingleOrDefault(t => t.Id == tabId);
			if (tab == null)
				return 0;

			return tab.Count;
		}

	}

}
