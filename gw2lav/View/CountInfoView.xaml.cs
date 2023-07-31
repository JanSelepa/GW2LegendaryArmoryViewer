using gw2lav.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace gw2lav.View {

	/// <summary>
	/// Interaction logic for CountInfoView.xaml
	/// </summary>
	/// 
	public partial class CountInfoView : UserControl {

		public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
			"IconSource",
			typeof(ImageSource),
			typeof(CountInfoView),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
		);

		public ImageSource IconSource {
			get { return (ImageSource)GetValue(IconSourceProperty); }
			set { SetValue(IconSourceProperty, value); }
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title",
			typeof(string),
			typeof(CountInfoView),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
		);

		public ImageSource Title {
			get { return (ImageSource)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty CountInfoProperty = DependencyProperty.Register(
			"CountInfo",
			typeof(CountInfo),
			typeof(CountInfoView),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
		);

		public ImageSource CountInfo {
			get { return (ImageSource)GetValue(CountInfoProperty); }
			set { SetValue(CountInfoProperty, value); }
		}

		public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
			"IsExpanded",
			typeof(bool),
			typeof(CountInfoView),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender)
		);

		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}

		public static readonly DependencyProperty AccentBrushProperty = DependencyProperty.Register(
			"AccentBrush",
			typeof(Brush),
			typeof(CountInfoView),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
		);

		public Brush AccentBrush {
			get { return (Brush)GetValue(AccentBrushProperty); }
			set { SetValue(AccentBrushProperty, value); }
		}

		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
			"Description",
			typeof(string),
			typeof(CountInfoView),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
		);

		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}

		public CountInfoView() {
			InitializeComponent();
		}

	}

}
