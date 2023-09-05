using NRadio.Core.Services;
using System.Threading.Tasks;

namespace NRadio.Core.Services
{
    public static class BackgroundWorkService
    {
        static BackgroundTaskService backgroundTaskService;

        public static async Task InitializeBackgroundTaskService()
        {
            var background = new BackgroundTaskService();
            backgroundTaskService = background;

            await backgroundTaskService.RegisterBackgroundTasksAsync();
        }
    }
}
