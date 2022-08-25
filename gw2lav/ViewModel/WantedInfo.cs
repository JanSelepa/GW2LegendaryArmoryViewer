using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace gw2lav.ViewModel {

	class WantedInfo : BindableBase {

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

		public ObservableCollection<Character> Characters { get; set; }

		private int _Count;
		public int Count {
			get { return _Count; }
			set { SetProperty(ref _Count, value); }
		}

		public WantedInfo() {
			Characters = new ObservableCollection<Character>();
			Count = 0;
		}

		public void Add(string characterName, string tabName, int tabId) {
			bool charFound = false;
			foreach (Character ch in Characters) {
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
				Application.Current.Dispatcher.Invoke(new Action(() => Characters.Add(newChar)));
			}
			Count++;
		}

	}

}
