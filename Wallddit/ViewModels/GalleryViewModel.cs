using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

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

            FillGallery();
        }

        public void FillGallery()
        {
            var wallpaperHistory = _dataService.GetWallpapers().Reverse<Wallpaper>().ToList();

            Groups = new List<GalleryGroup>
            {
                new GalleryGroup
                {
                    Title = "History",
                    Wallpapers = wallpaperHistory
                }
            };
        }
    }
}
