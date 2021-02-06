using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

using Wallddit.Core.Extensions;
using Wallddit.Core.Reddit;
using Wallddit.Core.Services;
using Wallddit.Core.Models;
using Wallddit.Helpers;

namespace Wallddit.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly WallpaperProvider _wallpaperProvider;
        private readonly string _wallpaperCacheFolder;

        [Reactive]
        private Wallpaper Wallpaper { get; set; }

        [ObservableAsProperty]
        public string WallpaperUrl { get; }

        [ObservableAsProperty]
        public bool IsThereWallpaper { get; }

        [ObservableAsProperty]
        public bool IsWallpaperSaved { get; }

        public ReactiveCommand<Unit, Unit> RefreshWallpaperCommand { get; }
        public ReactiveCommand<Unit, Unit> SetDesktopWallpaperCommand { get; }
        public ReactiveCommand<Unit, Unit> SwitchWallpaperSavedStateCommand { get; }

        public MainViewModel()
        {
            string appFolderPath = ApplicationData.Current.LocalFolder.Path;

            string dbPath = DataAccessHelper.GetDatabasePath();
            _wallpaperProvider = new WallpaperProvider(dbPath);

            const string WALLPAPER_CACHE_FOLDER_NAME = "wallpapers";
            string wallpaperCachePath = Path.Combine(appFolderPath, WALLPAPER_CACHE_FOLDER_NAME);
            _wallpaperCacheFolder = wallpaperCachePath;

            this.WhenAnyValue(x => x.Wallpaper.ImageUrl)
                .ToPropertyEx(this, x => x.WallpaperUrl);

            this.WhenAnyValue(x => x.Wallpaper)
                .Select(wallpaper => wallpaper != null)
                .ToPropertyEx(this, x => x.IsThereWallpaper);

            this.WhenAnyValue(x => x.Wallpaper.IsSaved)
                .ToPropertyEx(this, x => x.IsWallpaperSaved);

            RefreshWallpaperCommand = ReactiveCommand.CreateFromTask(RefreshWallpaperAsync);
            SetDesktopWallpaperCommand = ReactiveCommand.CreateFromTask(SetWallpaperAsync);
            SwitchWallpaperSavedStateCommand = ReactiveCommand.CreateFromTask(SwitchWallpaperSavedStateAsync);
        }

        public async Task RefreshWallpaperAsync()
        {
            Wallpaper = null;

            var wallpaper = await _wallpaperProvider.GetWallpaperAsync();

            Wallpaper = wallpaper;
        }

        public async Task SetWallpaperAsync()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                var imagePath = await HttpDataService.DownloadImageAsync(_wallpaperCacheFolder, Wallpaper.Id, Wallpaper.ImageUrl.ToUri());

                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(imagePath);
                UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
                await profileSettings.TrySetWallpaperImageAsync(imageFile);
            }
        }

        public async Task SwitchWallpaperSavedStateAsync()
        {
            var db = DataAccessHelper.GetDatabase();

            Wallpaper.IsSaved = !Wallpaper.IsSaved;

            var result = await db.UpdateWallpaperAsync(Wallpaper);

            if (!result)
            {
                Wallpaper.IsSaved = !Wallpaper.IsSaved;
            }
        }
    }
}
