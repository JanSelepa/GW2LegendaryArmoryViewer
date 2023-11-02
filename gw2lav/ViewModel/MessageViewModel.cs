using gw2lav.Service;
using System;

namespace gw2lav.ViewModel {

	class MessageViewModel : IDialogViewModel {

		public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

		public string Title { get; set; }
		public string Message { get; set; }
		public string PositiveText { get; set; }
		public string NegativeText { get; set; }

		public RelayCommand PositiveCommand { get; set; }
		public RelayCommand NegativeCommand { get; set; }

		public MessageViewModel(string title, string message, string positiveText, string negativeText) {
			Title = title;
			Message = message;
			PositiveText = positiveText;
			NegativeText = negativeText;

			PositiveCommand = new RelayCommand(OnPositive);
			NegativeCommand = new RelayCommand(OnNegative);
		}

		private void OnPositive() {
			CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
		}

		private void OnNegative() {
			CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
		}

	}

}
