using ReactiveUI;
using System;
using System.Reactive.Disposables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.WallpaperUrl,
                    view => view.wallpaperUrlTextBox.Text,
                    prop => prop ?? String.Empty)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.WallpaperImage,
                    view => view.wallpaperImage.Source)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsSetterAvailable,
                    view => view.setDesktopWallpaperButton.IsEnabled)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.GetWallpaperCommand,
                    view => view.getWallpaperButton)
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
    }
}
