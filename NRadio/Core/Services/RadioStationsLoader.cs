using NRadio.Core.API;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace NRadio.Core.Services
{
    public static class RadioStationsLoader
    {
        private const int MaxRecentStations = 20;

        private enum Countries
        {
            Ukraine,
            Poland,
            Italy,
            Germany,
            France,
            Russia,
            Belarus,
            Spain,
            CzechRepublic,
            UnitedKingdom,
        }

        private static StorageFolder _folder = ApplicationData.Current.LocalFolder;

        public static ConfigService Cfg { get; private set; }

        public static async Task Initialize()
        {
            StorageFile configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///config.json"));
            string configJson = await FileIO.ReadTextAsync(configFile);
            Cfg = JsonSerializer.Deserialize<ConfigService>(configJson);

            try
            {
                await LoadAllStationsToContainerAsync();
            }
            catch (FileNotFoundException)
            {
                await UpdateRadioStations();
            }
        }

        public static async Task UpdateRadioStations()
        {
            Filter options = new Filter
            {
                HasName = true,
                HasUrl = true,
                HasTag = false,
                HasFavicon = true,
                HasCountry = true,
                HasLanguage = true,
                MinBitrate = 64
            };
            await SaveFilteredStationsFromApiToContainerAsync(options);
            await SaveAllStationsToFile();
            await LoadAllStationsToContainerAsync();

            Debug.WriteLine("RadioStations was updated");
        }

        private static async Task SaveAllStationsToFile()
        {
            await SaveRadioStationsToFileAsync();
            await SaveRecentStationsToFileAsync();
        }

        private static async Task SaveRadioStationsToFileAsync() =>
            await _folder.SaveAsync(Cfg.RadioStationsFileName, RadioStationsContainer.AllStations);
        private static async Task SaveRecentStationsToFileAsync() =>
            await _folder.SaveAsync(Cfg.RecentStationsFileName, RadioStationsContainer.RecentsStations);

        private static async Task LoadAllStationsToContainerAsync()
        {
            RadioStationsContainer.AllStations = await LoadStationsFromFileAsync();
            ObservableCollection<RadioStation> RecentStations = await LoadRecentStationsFromFileAsync();

            if(RecentStations != null)
                RadioStationsContainer.RecentsStations = RecentStations;
            else
                RadioStationsContainer.RecentsStations = new ObservableCollection<RadioStation>();
        }

        private static async Task<ObservableCollection<RadioStation>> LoadStationsFromFileAsync() =>
            await _folder.ReadAsync<ObservableCollection<RadioStation>>(Cfg.RadioStationsFileName);
        private static async Task<ObservableCollection<RadioStation>> LoadRecentStationsFromFileAsync() =>
            await _folder.ReadAsync<ObservableCollection<RadioStation>>(Cfg.RecentStationsFileName);

        private static async Task SaveFilteredStationsFromApiToContainerAsync(Filter options) =>
            RadioStationsContainer.AllStations = FilterStations(await LoadStationsFromApiByAllCountryAsync(), options);

        private static async Task<ObservableCollection<RadioStation>> LoadStationsFromApiByAllCountryAsync()
        {
            List<RadioStation> allStations = new List<RadioStation>();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            foreach (var country in Enum.GetValues(typeof(Countries)).Cast<Countries>())
            {
                string response = await RadioBrowserAPI.GetStationsByCountryAsync(country.ToString());
                var data = JsonSerializer.Deserialize<RadioStation[]>(response, options);
                allStations.AddRange(data);
                await Task.Delay(200);
            }

            return new ObservableCollection<RadioStation>(allStations);
        }


        private static ObservableCollection<RadioStation> FilterStations(ObservableCollection<RadioStation> stations, Filter filter)
        {
            var filteredStations = stations.Where(station =>
                (!filter.HasName || !string.IsNullOrEmpty(station.Name)) &&
                (!filter.HasUrl || !string.IsNullOrEmpty(station.Url)) &&
                (!filter.HasTag || !string.IsNullOrEmpty(station.Tags)) &&
                (!filter.HasFavicon || !string.IsNullOrEmpty(station.Favicon)) &&
                (!filter.HasCountry || !string.IsNullOrEmpty(station.Country)) &&
                (!filter.HasLanguage || !string.IsNullOrEmpty(station.Language)) &&
                (station.Bitrate >= filter.MinBitrate)
            );

            return new ObservableCollection<RadioStation>(filteredStations);
        }

        public static async Task AddToLastRecentAsync(RadioStation station)
        {
            if (RadioStationsContainer.RecentsStations != null && RadioStationsContainer.RecentsStations.Count >= MaxRecentStations)
                RadioStationsContainer.RecentsStations.RemoveAt(0);
            if (RadioStationsContainer.RecentsStations.Contains(station))
                RadioStationsContainer.RecentsStations.Remove(station);

            RadioStationsContainer.RecentsStations.Add(station);
            await SaveRecentStationsToFileAsync();
        }
    }
}
