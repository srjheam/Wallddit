using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

using Wallddit.Core.Services;

namespace Wallddit.Helpers
{
    static class DataAccessHelper
    {
        public static SqliteDataService GetDatabase()
        {
            var dbPath = GetDatabasePath();
            var database = new SqliteDataService(dbPath);
            return database;
        }

        public static string GetDatabasePath()
        {
            string appFolderPath = ApplicationData.Current.LocalFolder.Path;

            const string WALLPAPER_DB_FILE_NAME = "wallpaper.db";
            string dbPath = Path.Combine(appFolderPath, WALLPAPER_DB_FILE_NAME);

            return dbPath;
        }

        public static async Task<StorageFolder> GetWallpapersFolderAsync()
        {
            const string WALLPAPER_CACHE_FOLDER_NAME = "wallpapers";

            return await ApplicationData.Current.LocalFolder.GetFolderAsync(WALLPAPER_CACHE_FOLDER_NAME);
        }
    }
}
