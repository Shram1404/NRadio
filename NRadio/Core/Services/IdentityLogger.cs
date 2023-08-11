using System.Diagnostics;
using Microsoft.IdentityModel.Abstractions;

namespace NRadio.Core.Services
{
    public class IdentityLogger : IIdentityLogger
    {
        private readonly EventLogLevel minLogLevel = EventLogLevel.LogAlways;

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
