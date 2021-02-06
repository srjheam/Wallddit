using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Wallddit.Core.Models;
using Wallddit.Core.Services;
using Wallddit.Helpers;
using Wallddit.Models;

namespace Wallddit.ViewModels
{
    public class GalleryViewModel : ReactiveObject
    {
        public readonly SqliteDataService _dataService;

        internal List<GalleryGroup> Groups { get; set; }

        public GalleryViewModel()
        {
            _dataService = DataAccessHelper.GetDatabase();
            Groups = new List<GalleryGroup>();

            FillGalleryAsync();
        }

        public async Task FillGalleryAsync()
        {
            var wallpaperHistory = (await _dataService.ReadAllWallpapersAsync()).Reverse<Wallpaper>().ToList();

            if (wallpaperHistory.Count == 0)
            {
                return;
            }

            var savedWallpapers = wallpaperHistory.Where(x => x.IsSaved).ToList();
            Groups.Add(new GalleryGroup
            {
                Title = "Saved",
                Wallpapers = savedWallpapers
            });

            Groups.Add(new GalleryGroup
            {
                Title = "History",
                Wallpapers = wallpaperHistory
            });
        }
    }
}
