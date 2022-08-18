using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gw2lav.View {

	class GrayableImage : Image {

		private bool _Grayscaled = false;

		static GrayableImage() {
			IsEnabledProperty.OverrideMetadata(typeof(GrayableImage), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsEnabledPropertyChanged)));
			SourceProperty.OverrideMetadata(typeof(GrayableImage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourcePropertyChanged)));
		}

		private static void OnIsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args) {
			GrayableImage grayableImage = (GrayableImage)source;
			ConvertToGray(grayableImage, grayableImage.Source, (bool)args.NewValue);
		}

		private static void OnSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args) {
			GrayableImage grayableImage = (GrayableImage)source;
			ConvertToGray(grayableImage, (ImageSource)args.NewValue, grayableImage.IsEnabled);
		}

		private static void ConvertToGray(GrayableImage grayableImage, ImageSource imageSource, bool isEnabled) {

			if (grayableImage == null || imageSource == null)
				return;

			if (!isEnabled) {

				// prevent multiple attempts of grayscaling
				if (grayableImage._Grayscaled) return;
				grayableImage._Grayscaled = true;

				// Get the source bitmap
				BitmapImage bitmapImage = new BitmapImage(new Uri(imageSource.ToString()));

				// Convert it to Gray
				grayableImage.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);

				grayableImage.Opacity = 0.5;

				// Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
				grayableImage.OpacityMask = new ImageBrush(bitmapImage);

			} else {

				// prevent multiple attempts of grayscaling
				if (!grayableImage._Grayscaled) return;
				grayableImage._Grayscaled = false;

				// Set the Source property to the original value.
				grayableImage.Source = ((FormatConvertedBitmap)imageSource).Source;

				grayableImage.Opacity = 1;

				// Reset the Opcity Mask
				grayableImage.OpacityMask = null;

			}

		}

	}

}
