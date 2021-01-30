using ReactiveUI;
using System;
using System.Reactive.Disposables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Wallddit.Core.Extensions;
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
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.WallpaperUrl,
                    view => view.wallpaperUrlTextBox.Text,
                    prop => prop ?? String.Empty)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.WallpaperUrl,
                    view => view.wallpaperImage.Source,
                    x => new BitmapImage(x.ToUri()))
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsSetterAvailable,
                    view => view.setDesktopWallpaperButton.IsEnabled)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.RefreshWallpaperCommand,
                    view => view.refreshWallpaperButton)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.SetDesktopWallpaperCommand,
                    view => view.setDesktopWallpaperButton)
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

        private void openCollectionButton_Click(object sender, RoutedEventArgs args) =>
            this.Frame.Navigate(typeof(GalleryPage));
    }
}
