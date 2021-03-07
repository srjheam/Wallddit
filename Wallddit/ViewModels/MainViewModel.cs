using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Wallddit.Core.Models;
using Wallddit.Helpers;

namespace Wallddit.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly WallpaperManager _wallpaperManager;

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
            _wallpaperManager = new WallpaperManager();
            UpdateWallpaperAsync();

            Observable.Interval(TimeSpan.FromMinutes(1)).Subscribe(async x => await UpdateWallpaperAsync());

            this.WhenAnyValue(x => x.Wallpaper)
                .Select(x => x.ImageUrl)
                .ToPropertyEx(this, x => x.WallpaperUrl);

            this.WhenAnyValue(x => x.Wallpaper)
                .Select(wallpaper => wallpaper != null)
                .ToPropertyEx(this, x => x.IsThereWallpaper);

            this.WhenAnyValue(x => x.Wallpaper)
                .Select(x => x.IsSaved)
                .ToPropertyEx(this, x => x.IsWallpaperSaved);

            RefreshWallpaperCommand = ReactiveCommand.CreateFromTask(RefreshWallpaper);
            SetDesktopWallpaperCommand = ReactiveCommand.CreateFromTask(async () => await _wallpaperManager.SetWallpaperAsDesktopWallpaperAsync());
            SwitchWallpaperSavedStateCommand = ReactiveCommand.CreateFromTask(SwitchWallpaperSavedStateAsync);
        }

        public async Task RefreshWallpaper()
        {
            var wallpaper = await _wallpaperManager.GetNextAsync();
            Wallpaper = wallpaper;
        }

        public async Task SwitchWallpaperSavedStateAsync()
        {
            var db = DataAccessHelper.GetDatabase();

            Wallpaper.IsSaved = !Wallpaper.IsSaved;

            var result = await db.UpdateWallpaperAsync(Wallpaper);

            if (result)
            {
                var tmp = Wallpaper;
                Wallpaper = new Wallpaper();
                Wallpaper = tmp;
            }
            else
            {
                Wallpaper.IsSaved = !Wallpaper.IsSaved;
            }
        }

        async Task UpdateWallpaperAsync()
        {
            Wallpaper = await _wallpaperManager.GetCurrentWallpaperAsync();
        }
    }
}
