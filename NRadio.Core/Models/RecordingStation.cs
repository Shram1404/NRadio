using System;

namespace NRadio.Core.Models
{
    public class RecordingStation
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
