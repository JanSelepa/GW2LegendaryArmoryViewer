using System.Collections.ObjectModel;

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
						ch.Tabs.Add(new Tab(tabName, tabId));
					}
					break;
				}
			}
			if (!charFound) {
				Character newChar = new Character(characterName);
				newChar.Count++;
				newChar.Tabs.Add(new Tab(tabName, tabId));
				Characters.Add(newChar);
			}
			Count++;
		}

		public void Add(WantedInfo wanted) {
			foreach (Character newChar in wanted.Characters) {
				bool newCharFound = false;
				foreach (Character oldChar in Characters) {
					if (newChar.Name == oldChar.Name) {
						newCharFound = true;
						oldChar.Count += newChar.Count;

						foreach (Tab newTab in newChar.Tabs) {
							bool newTabFound = false;
							foreach (Tab oldTab in oldChar.Tabs) {
								if (newTab.Id == oldTab.Id) {
									newTabFound = true;
									oldTab.Count += newTab.Count;
									break;
								}
							}
							if (!newTabFound) {
								oldChar.Tabs.Add(newTab);
							}
						}

						break;
					}
				}
				if (!newCharFound) {
					Characters.Add(newChar);
				}
			}
			Count += wanted.Count;
		}

	}

}
