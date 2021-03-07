using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

using Wallddit.Core.Services;
using Wallddit.Core.Extensions;
using Wallddit.Core.Models;
using Wallddit.Core.Reddit;
using Wallddit.Helpers;

namespace Wallddit
{
    sealed class WallpaperManager
    {
        private readonly SqliteDataService _dbAccess;

        public WallpaperProvider Provider { get; }

        public WallpaperManager()
        {
            _dbAccess = DataAccessHelper.GetDatabase();

            Provider = new WallpaperProvider();
        }

        public async Task<Wallpaper> GetCurrentWallpaperAsync()
        {
            var lastWallpaper = await _dbAccess.ReadAllWallpapersAsync();

            if (lastWallpaper.Count == 0)
            {
                return await GetNextAsync();
            }
            else
            {
                return lastWallpaper[lastWallpaper.Count - 1];
            }
        }

        public async Task<Wallpaper> GetNextAsync()
        {
            var denyList = (await _dbAccess.ReadAllWallpapersAsync()).Select(x => x.Id);

            var currentWallpaper = await Provider.GetWallpaperAsync(denyList);

            await _dbAccess.CreateWallpaperAsync(currentWallpaper);

            return currentWallpaper;
        }


        public async Task SetWallpaperAsDesktopWallpaperAsync()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                var currentWallpaper = await GetCurrentWallpaperAsync();
                var imagePath = await HttpDataService.DownloadImageAsync((await DataAccessHelper.GetWallpapersFolderAsync()).Path, currentWallpaper.Id, currentWallpaper.ImageUrl.ToUri());

                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(imagePath);
                UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
                await profileSettings.TrySetWallpaperImageAsync(imageFile);
            }
        }
    }
}
