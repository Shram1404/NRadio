using Microsoft.IdentityModel.Abstractions;
using System.Diagnostics;

namespace NRadio.Core.Services
{
    public class IdentityLogger : IIdentityLogger
    {
        private readonly EventLogLevel minLogLevel;

        public IdentityLogger(EventLogLevel minLogLevel = EventLogLevel.LogAlways)
        {
            this.minLogLevel = minLogLevel;
        }

        public bool IsEnabled(EventLogLevel eventLogLevel)
        {
            return eventLogLevel >= minLogLevel;
        }

        public void Log(LogEntry entry)
        {
            Debug.WriteLine($"MSAL: EventLogLevel: {entry.EventLogLevel}, Message: {entry.Message} ");
        }
    }
}
