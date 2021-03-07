using Humanizer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using Windows.UI.Xaml.Controls;

using Wallddit.Helpers;

namespace Wallddit.ViewModels
{
    public class WallpaperFlowViewModel : ReactiveObject
    {
        public List<Tuple<string, TimeSpan>> ComboBoxItems { get; private set; }

        [Reactive]
        public bool IsWallpaperFlowOn { get; set; }
        [Reactive]
        public int ComboBoxSelectedIndex { get; set; }

        public ReactiveCommand<Unit, Unit> WallpaperFlowSwitchToggleCommand { get; }

        public WallpaperFlowViewModel()
        {
            PopulateTimeTriggerComboBox();

            IsWallpaperFlowOn = WallpaperFlowHelper.IsOn;

            WallpaperFlowSwitchToggleCommand = ReactiveCommand.CreateFromTask(async () => await WallpaperFlowHelper.SwitchWallpaperFlowState());
        }

        public async void TaskTimeTriggerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxSelectedIndex == -1)
            {
                var settingsSelectedIndex = ComboBoxItems.FindIndex(x => x.Item2 == TimeSpan.FromMinutes(WallpaperFlowHelper.GetCurrentSettings().TriggerFreshnessTime));
                ComboBoxSelectedIndex = settingsSelectedIndex != -1 ? settingsSelectedIndex : 0;
            }

            await WallpaperFlowHelper.SetTriggerFreshnessTime((uint)Math.Floor((double)ComboBoxItems[ComboBoxSelectedIndex].Item2.TotalMinutes));
        }

        private void PopulateTimeTriggerComboBox()
        {
            ComboBoxItems = new List<Tuple<string, TimeSpan>>();
            var tsItems = new TimeSpan[] { TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30), TimeSpan.FromHours(1), TimeSpan.FromHours(6), TimeSpan.FromDays(1) };
            foreach (var ts in tsItems)
            {
                ComboBoxItems.Add(new Tuple<string, TimeSpan>(ts.Humanize(), ts));
            }
        }
    }
}
