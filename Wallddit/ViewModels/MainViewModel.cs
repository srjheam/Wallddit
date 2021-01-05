﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

using Wallddit.Core.Reddit;
using Wallddit.Core.Services;

namespace Wallddit.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly StorageFolder _appFolder;
        private readonly string _wallpaperFolder;

        private string _wallpaperUrl = String.Empty;
        public string WallpaperUrl
        {
            get => _wallpaperUrl;
            set => this.RaiseAndSetIfChanged(ref _wallpaperUrl, value);
        }

        private BitmapImage _wallpaperSource = default;
        public BitmapImage WallpaperSource
        {
            get => _wallpaperSource;
            set => this.RaiseAndSetIfChanged(ref _wallpaperSource, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isSetterAvailable;
        public bool IsSetterAvailable => _isSetterAvailable.Value;

        public ReactiveCommand<Unit, Unit> GetWallpaperCommand { get; }
        public ReactiveCommand<Unit, Unit> SetDesktopWallpaperCommand { get; }

        public APIWrapper APIWrapper { get; set; } = new APIWrapper();

        public MainViewModel()
        {
            const string WALLPAPER_FOLDER_NAME = "wallpapers";
            _appFolder = ApplicationData.Current.LocalFolder;
            _wallpaperFolder = Path.Combine(_appFolder.Path, WALLPAPER_FOLDER_NAME);
            if (!Directory.Exists(_wallpaperFolder))
            {
                _appFolder.CreateFolderAsync(WALLPAPER_FOLDER_NAME).AsTask().GetAwaiter().GetResult();
            }

            _isSetterAvailable = this
                .WhenAnyValue(x => x.WallpaperSource)
                .Select(wallpaper => wallpaper != null)
                .ToProperty(this, x => x.IsSetterAvailable);

            GetWallpaperCommand = ReactiveCommand.CreateFromTask(GetWallpaperAsync);
            SetDesktopWallpaperCommand = ReactiveCommand.CreateFromTask(SetWallpaperAsync);
        }

        public async Task GetWallpaperAsync()
        {
            var apiCallResponse = await APIWrapper.GetHotPostsAsync("wallpaper", new Dictionary<string, string>
            {
                ["limit"] = "1"
            });
            WallpaperUrl = apiCallResponse.data.children[0].data.url_overridden_by_dest;

            var wallpaperName = (string)apiCallResponse.data.children[0].data.name;
            string wallpaperPath = null;
            if (!IsWallpaperDownloaded(wallpaperName))
            {
                wallpaperPath = await HttpDataService.DownloadImageAsync(_wallpaperFolder, wallpaperName, new Uri(WallpaperUrl));
            }
            WallpaperSource = new BitmapImage(new Uri(wallpaperPath ?? GetDownloadedWallpaper(wallpaperName)));
        }

        public async Task SetWallpaperAsync()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                StorageFile wallpaper = await StorageFile.GetFileFromPathAsync(WallpaperSource.UriSource.AbsoluteUri);
                UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
                await profileSettings.TrySetWallpaperImageAsync(wallpaper);
            }
        }

        private string GetDownloadedWallpaper(string wallpaperName)
        {
            var wallpapers = Directory.GetFiles(_wallpaperFolder, $"{wallpaperName}.*");
            if (wallpapers.Length > 0)
            {
                return wallpapers[0];
            }
            return null;
        }

        private bool IsWallpaperDownloaded(string wallpaperName)
        {
            var wallpapersCount = Directory.GetFiles(_wallpaperFolder, $"{wallpaperName}.*");
            return wallpapersCount.Length > 0;
        }
    }
}
