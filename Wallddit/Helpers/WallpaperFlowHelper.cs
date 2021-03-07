using System;
using System.Threading.Tasks;
using Windows.Storage;

using Wallddit.BackgroundTasks;
using Wallddit.Models;

namespace Wallddit.Helpers
{
    public static class WallpaperFlowHelper
    {
        private const string ContainerName = "WallpaperFlowTask";
        private static ApplicationDataContainer SettingsContainer => ApplicationData.Current.LocalSettings.CreateContainer(ContainerName, ApplicationDataCreateDisposition.Always);

        public static bool IsOn => new WallpaperFlowTask().IsRegistered;

        public static async Task SwitchWallpaperFlowState()
        {
            var bgTask = new WallpaperFlowTask();

            if (!IsOn)
            {
                await bgTask.Register();
            }
            else
            {
                bgTask.Unregister();
            }

            SettingsContainer.Values[nameof(WallpaperFlowSettings.IsOn)] = IsOn;
        }

        public static async Task SetTriggerFreshnessTime(uint minutes)
        {
            if (minutes < 15)
            {
                throw new ArgumentOutOfRangeException("Freshness Time must be greater than 15 minutes.");
            }

            if (minutes != (SettingsContainer.Values[nameof(WallpaperFlowSettings.TriggerFreshnessTime)] as uint? ?? 15))
            {
                SettingsContainer.Values[nameof(WallpaperFlowSettings.TriggerFreshnessTime)] = minutes;
                await new WallpaperFlowTask().Reload();
            }
        }

        public static WallpaperFlowSettings GetCurrentSettings()
        {
            var result = new WallpaperFlowSettings();

            object settingsIsOn = null;
            if (SettingsContainer.Values.ContainsKey(nameof(result.IsOn)))
            {
                settingsIsOn = SettingsContainer.Values[nameof(result.IsOn)];
            }
            result.IsOn = (settingsIsOn as bool?) ?? false;

            object settingsFressnessTime = null;
            if (SettingsContainer.Values.ContainsKey(nameof(result.TriggerFreshnessTime)))
            {
                settingsFressnessTime = SettingsContainer.Values[nameof(result.TriggerFreshnessTime)];
            }
            result.TriggerFreshnessTime = (settingsFressnessTime as uint?) ?? 15;

            return result;
        }

        public static void Initialize()
        {
            var bgTask = new WallpaperFlowTask();
            if (GetCurrentSettings().IsOn)
            {
                bgTask.Register();
            }
            else
            {
                bgTask.Unregister();
            }
        }
    }
}
