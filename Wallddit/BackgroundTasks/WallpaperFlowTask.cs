using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

using Wallddit.Helpers;

namespace Wallddit.BackgroundTasks
{
    sealed class WallpaperFlowTask
    {
        public string TaskName => GetType().Name;
        public KeyValuePair<Guid, IBackgroundTaskRegistration> TaskRegistration => BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == TaskName);

        public bool IsRegistered => !(TaskRegistration.Value is null);

        public async Task Register()
        {
            if (IsRegistered)
            {
                return;
            }

            var builder = new BackgroundTaskBuilder()
            {
                Name = TaskName,
                IsNetworkRequested = true
            };

            builder.SetTrigger(new TimeTrigger(WallpaperFlowHelper.GetCurrentSettings().TriggerFreshnessTime, false));

            builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));

            await BackgroundExecutionManager.RequestAccessAsync();
            builder.Register();
        }

        public void Unregister()
        {
            if (!IsRegistered)
            {
                return;
            }

            TaskRegistration.Value.Unregister(false);
        }

        public async Task Reload()
        {
            if (!IsRegistered)
            {
                return;
            }

            Unregister();
            await Register();
        } 

        public Task Run(IBackgroundTaskInstance instance)
        {
            if (instance == null)
            {
                return null;
            }

            var deferral = instance.GetDeferral();

            return Task.Run(async () =>
            {
                var wallpaperManager = new WallpaperManager();
                var wallpaper = await wallpaperManager.GetNextAsync();
                instance.Progress = 33;
                await wallpaperManager.SetWallpaperAsDesktopWallpaperAsync();
                instance.Progress = 100;
                deferral.Complete();
            });
        }
    }
}
