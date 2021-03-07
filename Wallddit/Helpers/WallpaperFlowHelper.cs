using System.Threading.Tasks;
using Windows.Storage;

using Wallddit.BackgroundTasks;

namespace Wallddit.Helpers
{
    public static class WallpaperFlowHelper
    {
        private const string SettingsKey = "IsWallpaperFlowOn";

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

            ApplicationData.Current.LocalSettings.Values[SettingsKey] = IsOn;
        }

        public static void Initialize()
        {
            var bgTask = new WallpaperFlowTask();
            if (IsOn)
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
