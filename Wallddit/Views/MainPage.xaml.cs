using ReactiveUI;
using System.Reactive.Disposables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using Wallddit.ViewModels;

namespace Wallddit.Views
{
    public sealed partial class MainPage : Page, IViewFor<MainViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(MainViewModel), typeof(MainPage), null);

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel,
                    viewModel => viewModel.GetWallpaperCommand,
                    view => view.getWallpaperButton)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.WallpaperUrl,
                    view => view.wallpaperUrlTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.WallpaperSource,
                    view => view.wallpaperImage.Source,
                    this.BitmapImageToImageSource,
                    this.ImageSourceToBitmapImage)
                    .DisposeWith(disposables);
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel)value;
        }

        public MainViewModel ViewModel
        {
            get => (MainViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private ImageSource BitmapImageToImageSource(BitmapImage bm)
        {
            return bm;
        }

        private BitmapImage ImageSourceToBitmapImage(ImageSource imageSource)
        {
            return imageSource as BitmapImage;
        }
    }
}
