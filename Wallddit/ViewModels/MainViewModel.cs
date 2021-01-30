﻿using ReactiveUI;
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
        public bool IsSetterAvailable { get; }

        public ReactiveCommand<Unit, Unit> RefreshWallpaperCommand { get; }
        public ReactiveCommand<Unit, Unit> SetDesktopWallpaperCommand { get; }

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
                .ToPropertyEx(this, x => x.IsSetterAvailable);

            RefreshWallpaperCommand = ReactiveCommand.CreateFromTask(RefreshWallpaperAsync);
            SetDesktopWallpaperCommand = ReactiveCommand.CreateFromTask(SetWallpaperAsync);

            RefreshWallpaperAsync();
        }

        public async Task RefreshWallpaperAsync()
        {
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
    }
}
