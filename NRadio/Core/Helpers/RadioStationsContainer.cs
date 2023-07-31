using NRadio.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NRadio.Core.Helpers
{
    public static class RadioStationsContainer
    {
        public static ObservableCollection<RadioStation> AllStations { get; set; } = new ObservableCollection<RadioStation>();
        public static ObservableCollection<RadioStation> RecentsStations { get; set; } = new ObservableCollection<RadioStation>();
    }
}
