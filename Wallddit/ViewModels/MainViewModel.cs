using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

using Wallddit.Core;
using Wallddit.Core.Reddit;
using Wallddit.Core.Services;

namespace Wallddit.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly StorageFolder _appFolder;
        private readonly string _wallpaperFolder;
        private readonly WallpaperProvider _wallpaperProvider;

        [Reactive]
        private Wallpaper Wallpaper { get; set; }

        [ObservableAsProperty]
        public string WallpaperUrl { get; }

        [ObservableAsProperty]
        public BitmapImage WallpaperImage { get; }

        [ObservableAsProperty]
        public bool IsSetterAvailable { get; }

        public ReactiveCommand<Unit, Unit> GetWallpaperCommand { get; }
        public ReactiveCommand<Unit, Unit> SetDesktopWallpaperCommand { get; }

        public MainViewModel()
        {
            const string WALLPAPER_FOLDER_NAME = "wallpapers";
            _appFolder = ApplicationData.Current.LocalFolder;
            _wallpaperFolder = Path.Combine(_appFolder.Path, WALLPAPER_FOLDER_NAME);
            if (!Directory.Exists(_wallpaperFolder))
            {
                _appFolder.CreateFolderAsync(WALLPAPER_FOLDER_NAME).AsTask().GetAwaiter().GetResult();
            }
            _wallpaperProvider = new WallpaperProvider();

            this.WhenAnyValue(x => x.Wallpaper.Image.UriSource.AbsoluteUri)
                .ToPropertyEx(this, x => x.WallpaperUrl);

            this.WhenAnyValue(x => x.Wallpaper.Image)
                .ToPropertyEx(this, x => x.WallpaperImage);

            this.WhenAnyValue(x => x.Wallpaper)
                .Select(wallpaper => wallpaper != null)
                .ToPropertyEx(this, x => x.IsSetterAvailable);

            GetWallpaperCommand = ReactiveCommand.CreateFromTask(GetWallpaperAsync);
            SetDesktopWallpaperCommand = ReactiveCommand.CreateFromTask(SetWallpaperAsync);
        }

        public async Task GetWallpaperAsync()
        {
            var wallpaper = await _wallpaperProvider.GetFreshWallpaperAsync();

            Wallpaper = wallpaper;
        }

        public async Task SetWallpaperAsync()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                var imagePath = await HttpDataService.DownloadImageAsync(_wallpaperFolder, Wallpaper.Id, Wallpaper.Image.UriSource);
                
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(imagePath);
                UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
                await profileSettings.TrySetWallpaperImageAsync(imageFile);
            }
        }
    }
}
