using gw2lav.ViewModel;
using System;
using System.Windows;

namespace gw2lav.View {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			DialogService dialogService = new DialogService(this);
			dialogService.Register<SettingsViewModel, SettingsWindow>();
			dialogService.Register<InfoViewModel, InfoWindow>();

			StateChanged += onStateChangedWindow;

			DataContext = new LegendaryViewModel(dialogService);
			InitializeComponent();
		}

		private async void OnLoaded(object sender, RoutedEventArgs e) {
			await ((LegendaryViewModel)DataContext).LoadDataAsync();
		}

		private void onMinimizeClick(object sender, RoutedEventArgs e) {
			WindowState = WindowState.Minimized;
		}

		private void onMaximizeClick(object sender, RoutedEventArgs e) {
			WindowState = WindowState.Maximized;
		}

		private void onRestoreClick(object sender, RoutedEventArgs e) {
			WindowState = WindowState.Normal;
		}

		private void onCloseClick(object sender, RoutedEventArgs e) {
			Close();
		}

		private void onStateChangedWindow(object sender, EventArgs args) {
			// fix for maximized window overlapping the screen
			if (WindowState == WindowState.Maximized) {
				double scale = PresentationSource.FromVisual((Window)sender).CompositionTarget.TransformToDevice.M11;
				root.Margin = new Thickness(
					(SystemParameters.WindowNonClientFrameThickness.Left + SystemParameters.WindowResizeBorderThickness.Left - 1) / scale/** 96d / (int)typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null)*/,
					(SystemParameters.WindowNonClientFrameThickness.Top + SystemParameters.WindowResizeBorderThickness.Top - SystemParameters.CaptionHeight - 2) / scale,
					(SystemParameters.WindowNonClientFrameThickness.Right + SystemParameters.WindowResizeBorderThickness.Right - 1) / scale,
					(SystemParameters.WindowNonClientFrameThickness.Bottom + SystemParameters.WindowResizeBorderThickness.Bottom - 1) / scale
				);
			} else
				root.Margin = new Thickness(0);
		}

	}

}
