using System;
using System.Collections.Generic;
using System.Windows;

namespace gw2lav {

	public interface IDialogWindow {
		Window Owner { get; set; }
		object DataContext { get; set; }
		bool? DialogResult { get; set; }
		bool? ShowDialog();
		void Close();
	}

	public interface IDialogViewModel {
		event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
	}

	public class DialogCloseRequestedEventArgs : EventArgs {
		public bool? DialogResult { get; }
		public DialogCloseRequestedEventArgs(bool? dialogResult) {
			DialogResult = dialogResult;
		}
	}

	public interface IDialogService {
		void Register<TViewModel, TView>()
			where TViewModel : IDialogViewModel
			where TView : IDialogWindow;

		bool? ShowDialog<TViewModel>(TViewModel viewModel)
			where TViewModel : IDialogViewModel;
	}

	class DialogService : IDialogService {

		private Dictionary<Type, Type> _Mappings;

		private Window _Owner;

		public DialogService(Window owner) {
			_Owner = owner;
			_Mappings = new Dictionary<Type, Type>();
		}

		public void Register<TViewModel, TView>() where TViewModel : IDialogViewModel where TView : IDialogWindow {
			_Mappings.Add(typeof(TViewModel), typeof(TView));
		}

		public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogViewModel {
			Type viewType = _Mappings[typeof(TViewModel)];

			IDialogWindow dialog = (IDialogWindow)Activator.CreateInstance(viewType);

			EventHandler<DialogCloseRequestedEventArgs> handler = null;
			handler = (sender, e) => {
				viewModel.CloseRequested -= handler;
				if (e.DialogResult.HasValue)
					dialog.DialogResult = e.DialogResult;
				else
					dialog.Close();
			};
			viewModel.CloseRequested += handler;

			dialog.DataContext = viewModel;
			dialog.Owner = _Owner;

			return dialog.ShowDialog();
		}

	}

}
