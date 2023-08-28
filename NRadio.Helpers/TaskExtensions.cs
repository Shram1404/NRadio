using System.Threading.Tasks;

namespace NRadio.Helpers
{
    public static class TaskExtensions
    {
        public static void FireAndForget(this Task task)
        {
            // This method allows you to call an async method without awaiting it
        }
    }
}