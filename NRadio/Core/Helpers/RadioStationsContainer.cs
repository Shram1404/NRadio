using NRadio.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRadio.Core.Helpers
{
    public static class RadioStationsContainer
    {
        public static List<RadioStation> AllStations { get; set; } = new List<RadioStation>();
        public static List<RadioStation> RecentsStations { get; set; } = new List<RadioStation>();
    }
}
