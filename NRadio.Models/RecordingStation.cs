using System;
using System.Xml.Linq;

namespace NRadio.Models
{
    public class RecordingStation
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is RecordingStation other)
            {
                return Name == other.Name && Uri == other.Uri;
            }

            return false;
        }
    }
}
