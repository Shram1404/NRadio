using NRadio.BackgroundTasks;
using NRadio.Core.Services;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace NRadio.Core.BackgroundTasks
{
    public class UpdateRadioStationsTask : BackgroundTask
    {
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

            builder.SetTrigger(new TimeTrigger(60, false));
            builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            builder.Register();
        }

        public override async Task RunAsyncInternal(IBackgroundTaskInstance taskInstance)
        {
            await RadioStationsLoader.UpdateRadioStations();
            await Task.CompletedTask;
        }
        public override async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            await Task.CompletedTask;
        }
    }
}
