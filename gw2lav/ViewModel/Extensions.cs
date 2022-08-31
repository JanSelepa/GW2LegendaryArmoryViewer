using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace gw2lav.ViewModel {

	public static class ObservableCollection {
		public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector, bool ascending) {
			if (ascending) {
				List<TSource> sortedList = source.OrderBy(keySelector).ToList();
				source.Clear();
				foreach (var sortedItem in sortedList) {
					source.Add(sortedItem);
				}
			} else {
				List<TSource> sortedList = source.OrderByDescending(keySelector).ToList();
				source.Clear();
				foreach (var sortedItem in sortedList) {
					source.Add(sortedItem);
				}
			}
		}
	}

}
