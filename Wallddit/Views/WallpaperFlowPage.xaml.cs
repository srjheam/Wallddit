using ReactiveUI;
using System.Reactive.Disposables;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Wallddit.ViewModels;

namespace Wallddit.Views
{
    public sealed partial class WallpaperFlowPage : Page, IViewFor<WallpaperFlowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(WallpaperFlowViewModel), typeof(MainPage), null);

        public WallpaperFlowPage()
        {
            this.InitializeComponent();
            ViewModel = new WallpaperFlowViewModel();

            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += CurrentView_BackRequested;

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel,
                    viewModel => viewModel.IsWallpaperFlowOn,
                    view => view.WallpaperFlowToggleSwitch.IsOn)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.WallpaperFlowSwitchToggleCommand,
                    view => view.WallpaperFlowToggleSwitch)
                    .DisposeWith(disposables);
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (WallpaperFlowViewModel)value;
        }

        public WallpaperFlowViewModel ViewModel
        {
            get => (WallpaperFlowViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private void GoToHomeButton_Click(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
