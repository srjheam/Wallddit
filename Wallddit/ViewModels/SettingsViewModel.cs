using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

using System.Threading.Tasks;
using System.Reactive;
using System;
using System.Reactive.Linq;
using Wallddit.Helpers;

namespace Wallddit.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        private ElementTheme _currentTheme;
        public readonly string AppDisplayName;
        public readonly string AppVersion;

        [ObservableAsProperty]
        public int SelectedTheme { get; }

        public ReactiveCommand<object, Unit> SwitchThemeCommand { get; }
        public ReactiveCommand<Unit, Unit> LaunchFeedbackHubCommand { get; }
        public ReactiveCommand<Unit, Unit> LaunchGitHubRepoCommand { get; }

        public Visibility FeedbackLinkVisibility => Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported() ? Visibility.Visible : Visibility.Collapsed;

        public SettingsViewModel()
        {
            _currentTheme = ThemeHelper.Theme;

            AppDisplayName = "Wallddit";
            AppVersion = GetVersion();

            this.WhenAnyValue(x => x._currentTheme)
                .Select(theme =>
                {
                    switch (theme)
                    {
                        case ElementTheme.Default:
                            return 2;
                        default:
                            return (int)theme - 1;
                    }
                })
                .ToPropertyEx(this, x => x.SelectedTheme);

            SwitchThemeCommand = ReactiveCommand.Create<object>(x => SwitchTheme(x));
            LaunchFeedbackHubCommand = ReactiveCommand.CreateFromTask(LaunchFeedbackHub);
            LaunchGitHubRepoCommand = ReactiveCommand.CreateFromTask(LaunchGitHubRepo);
        }

        public void SwitchTheme(object param)
        {
            _currentTheme = (ElementTheme)param;
            if (_currentTheme != ThemeHelper.Theme)
            {
                ThemeHelper.Theme = _currentTheme;
            }
        }

        public async Task LaunchFeedbackHub()
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        public async Task LaunchGitHubRepo()
        {
            Uri gitHubRepo = new Uri("https://github.com/srjheam/Wallddit");

            await Windows.System.Launcher.LaunchUriAsync(gitHubRepo);
        }

        private string GetVersion()
        {
            var version = Package.Current.Id.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
