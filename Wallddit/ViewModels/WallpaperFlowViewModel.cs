using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Wallddit.Helpers;

namespace Wallddit.ViewModels
{
    public class WallpaperFlowViewModel : ReactiveObject
    {
        [Reactive]
        public bool IsWallpaperFlowOn { get; set; }

        public ReactiveCommand<Unit, Unit> WallpaperFlowSwitchToggleCommand { get; }

        public WallpaperFlowViewModel()
        {
            IsWallpaperFlowOn = WallpaperFlowHelper.IsOn;

            WallpaperFlowSwitchToggleCommand = ReactiveCommand.CreateFromTask(async () => await WallpaperFlowHelper.SwitchWallpaperFlowState());
        }
    }
}
