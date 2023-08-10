using NRadio.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NRadio.Core.Helpers
{
    public static class RadioStationsContainer
    {
        public static List<RadioStation> AllStations { get; set; } = new List<RadioStation>();
        public static List<RadioStation> RecentStations { get; set; } = new List<RadioStation>();
        public static List<RadioStation> FavoriteStations { get; set; } = new List<RadioStation>();
        public static List<RadioStation> PremiumStations { get; set; } = new List<RadioStation>();
    }
}
