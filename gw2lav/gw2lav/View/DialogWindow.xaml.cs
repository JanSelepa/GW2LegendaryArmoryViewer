using System.Windows;

namespace gw2lav.View {

	public partial class DialogWindow : Window {

		public DialogWindow() {
			InitializeComponent();
			Owner = Application.Current.MainWindow;
		}

		public void SetContent(UIElement content) {
			borderContent.Child = content;
		}

		public void SetPositiveButton(string text) {
			panelButtons.Visibility = Visibility.Visible;
			btnPositive.Visibility = Visibility.Visible;
			btnPositive.Content = text;
		}

		public void SetNegativeButton(string text) {
			panelButtons.Visibility = Visibility.Visible;
			btnNegative.Visibility = Visibility.Visible;
			btnNegative.Content = text;
		}

		private void OnClickPositive(object sender, RoutedEventArgs e) {
			DialogResult = true;
		}

		private void OnClickNegative(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}

	}

}
