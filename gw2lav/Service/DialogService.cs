using System;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace gw2lav.Service {

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
		bool? ShowDialog<TViewModel>(TViewModel viewModel)
			where TViewModel : IDialogViewModel;
	}

	class DialogService : IDialogService {

		public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogViewModel {
			Type viewType = ResolveViewType(typeof(TViewModel));

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
			dialog.Owner = Application.Current.MainWindow;

			return dialog.ShowDialog();
		}

		private Type ResolveViewType(Type viewModelType) {
			string viewName = viewModelType.FullName.Replace(".ViewModel.", ".View.");
			viewName = viewName.Substring(0, viewName.Length - "ViewModel".Length);
			string viewType = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, "Window", viewModelType.GetTypeInfo().Assembly.FullName);
			return Type.GetType(viewType);
		}

	}

}
