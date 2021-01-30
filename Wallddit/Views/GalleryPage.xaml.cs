using ReactiveUI;
using System.Collections.Generic;
using Wallddit.Models;
using Wallddit.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wallddit.Views
{
    /// <summary>
    /// The gallery page containing the wallpaper history and user favorites.
    /// </summary>
    public sealed partial class GalleryPage : Page, IViewFor<GalleryViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(GalleryViewModel), typeof(GalleryPage), null);

        internal List<GalleryGroup> Groups => ViewModel.Groups;

        public GalleryPage()
        {
            this.InitializeComponent();
            ViewModel = new GalleryViewModel();

            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += CurrentView_BackRequested;
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (GalleryViewModel)value;
        }

        public GalleryViewModel ViewModel
        {
            get => (GalleryViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
