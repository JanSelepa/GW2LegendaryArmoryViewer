using gw2lav.ViewModel;
using System.Windows;

namespace gw2lav.View {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			DialogService dialogService = new DialogService(this);
			dialogService.Register<SettingsViewModel, SettingsWindow>();

			DataContext = new LegendaryViewModel(dialogService);
			InitializeComponent();
		}

		private async void OnLoaded(object sender, RoutedEventArgs e) {
			await ((LegendaryViewModel)DataContext).LoadDataAsync();
		}

	}

}
