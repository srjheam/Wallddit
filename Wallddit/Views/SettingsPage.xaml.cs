using ReactiveUI;
using System;
using System.Reactive.Disposables;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Wallddit.ViewModels;

namespace Wallddit.Views
{
    public sealed partial class SettingsPage : Page, IViewFor<SettingsViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(SettingsViewModel), typeof(SettingsPage), null);

        public SettingsPage()
        {
            this.InitializeComponent();
            ViewModel = new SettingsViewModel();

            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += CurrentView_BackRequested;

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.SelectedTheme,
                    view => view.SwitchThemeRadioButtons.SelectedIndex)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.AppDisplayName,
                    view => view.AppDisplayName.Text,
                    prop => prop ?? String.Empty)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                   viewModel => viewModel.AppVersion,
                   view => view.AppVersion.Text,
                   prop => prop ?? String.Empty)
                   .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                   viewModel => viewModel.FeedbackLinkVisibility,
                   view => view.FeedbackHubButton.Visibility)
                   .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.SwitchThemeCommand,
                    view => view.SwitchThemeRadioButtons.Items[0])
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.SwitchThemeCommand,
                    view => view.SwitchThemeRadioButtons.Items[1])
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.SwitchThemeCommand,
                    view => view.SwitchThemeRadioButtons.Items[2])
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.LaunchFeedbackHubCommand,
                    view => view.FeedbackHubButton)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.LaunchGitHubRepoCommand,
                    view => view.GitHubRepoButton)
                    .DisposeWith(disposables);
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (SettingsViewModel)value;
        }

        public SettingsViewModel ViewModel
        {
            get => (SettingsViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
