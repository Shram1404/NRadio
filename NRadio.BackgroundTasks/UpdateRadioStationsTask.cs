using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace NRadio.Core.BackgroundTasks
{
    public class UpdateRadioStationsTask : BackgroundTask
    {
        public Type StationsLoader { get; set; }

        public UpdateRadioStationsTask(Type stationsLoader)
        {
            StationsLoader = stationsLoader;
        }

        public override void Register()
        {
            var taskName = GetType().Name;
            var taskRegistration = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(i => i.Name == taskName);

            if (taskRegistration != null)
            {
                return;
            }

            var builder = new BackgroundTaskBuilder
            {
                Name = taskName
            };

            builder.SetTrigger(new TimeTrigger(360, false));
            builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            builder.Register();
        }

        public override async Task RunAsyncInternal(IBackgroundTaskInstance taskInstance)
        {
            var methodInfo = StationsLoader.GetMethod("UpdateRadioStationsAsync");
            if (methodInfo != null)
            {
                var task = (Task)methodInfo.Invoke(null, null);
                await task;
            }
            await Task.CompletedTask;
        }
        public override async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            await Task.CompletedTask;
        }
    }
}
