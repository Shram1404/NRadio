using NRadio.Core.Models;
using System.Collections.ObjectModel;

namespace NRadio.Core.Helpers
{
    public static class RadioStationsContainer
    {
        public static ObservableCollection<RadioStation> AllStations { get; set; } = new ObservableCollection<RadioStation>();
        public static ObservableCollection<RadioStation> RecentStations { get; set; } = new ObservableCollection<RadioStation>();
        public static ObservableCollection<RadioStation> FavoriteStations { get; set; } = new ObservableCollection<RadioStation>();
        public static ObservableCollection<RadioStation> PremiumStations { get; set; } = new ObservableCollection<RadioStation>();
    }
}
