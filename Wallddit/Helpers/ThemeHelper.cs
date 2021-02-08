using System;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI;

namespace Wallddit.Helpers
{
    public static class ThemeHelper
    {
        private const string SettingsKey = "SelectedAppTheme";
        private static Window CurrentApplicationWindow;
        private static UISettings uiSettings;

        public static bool IsDarkTheme
        {
            get
            {
                if (Theme == ElementTheme.Default)
                {
                    return Application.Current.RequestedTheme == ApplicationTheme.Dark;
                }
                return Theme == ElementTheme.Dark;
            }
        }

        public static ElementTheme Theme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }

                ApplicationData.Current.LocalSettings.Values[SettingsKey] = value.ToString();
                UpdateSystemCaptionButtonColors();
            }
        }

        public static void Initialize()
        {
            // Save reference as this might be null when the user is in another app
            CurrentApplicationWindow = Window.Current;
            LoadThemeFromSettings();

            // Registering to color changes, thus we notice when user changes theme system wide
            uiSettings = new UISettings();
            uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
        }

        public static void UpdateSystemCaptionButtonColors()
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            if (IsDarkTheme)
            {
                titleBar.ButtonForegroundColor = Colors.White;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.Black;
            }
        }

        private static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            // Make sure we have a reference to our window so we dispatch a UI change
            if (CurrentApplicationWindow != null)
            {
                // Dispatch on UI thread so that we have a current appbar to access and change
                CurrentApplicationWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    UpdateSystemCaptionButtonColors();
                });
            }
        }

        private static void LoadThemeFromSettings()
        {
            string savedTheme = ApplicationData.Current.LocalSettings.Values[SettingsKey]?.ToString();

            if (savedTheme != null)
            {
                Theme = Enum.Parse<ElementTheme>(savedTheme);
            }
        }
    }
}
