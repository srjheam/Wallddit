using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

using Wallddit.Core.Reddit;

namespace Wallddit.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private string _wallpaperUrl = String.Empty;
        public string WallpaperUrl
        {
            get => _wallpaperUrl;
            set => this.RaiseAndSetIfChanged(ref _wallpaperUrl, value);
        }

        public ReactiveCommand<Unit, Unit> GetWallpaperCommand { get; }

        public MainViewModel()
        {
            GetWallpaperCommand = ReactiveCommand.CreateFromTask(GetWallpaper);
        }

        public async Task GetWallpaper()
        {
            var imageUrl = await new APIWrapper().GetHotPostsAsync("wallpaper", new Dictionary<string, string>
            {
                ["limit"] = "1"
            });

            WallpaperUrl = imageUrl.data.children[0].data.url_overridden_by_dest;
        }
    }
}
