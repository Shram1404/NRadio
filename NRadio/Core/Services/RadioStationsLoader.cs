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
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace NRadio.Core.Services
{
    public static class RadioStationsLoader
    {
        private const int MaxRecentStations = 20;

        private static readonly StorageFolder folder = ApplicationData.Current.LocalFolder;

        public static ConfigService Cfg { get; private set; }

        public static async Task Initialize()
        {
            var configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///config.json"));
            string configJson = await FileIO.ReadTextAsync(configFile);
            Cfg = JsonSerializer.Deserialize<ConfigService>(configJson);

            try
            {
                await LoadAllFromFileToContainerAsync();
            }
            catch (FileNotFoundException)
            {
                await UpdateRadioStations();
            }
        }

        public static async Task UpdateRadioStations()
        {
            var options = new Filter
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
            await SavePremiumFromSomewhereToContainerASync();
            await SaveAllStationsToFile();
            await LoadAllFromFileToContainerAsync();

            Debug.WriteLine("RadioStations was updated");
        }

        public static async Task AddToLastRecentAsync(RadioStation station)
        {
            if (RadioStationsContainer.RecentStations != null && RadioStationsContainer.RecentStations.Count >= MaxRecentStations)
            {
                RadioStationsContainer.RecentStations.Remove(RadioStationsContainer.RecentStations.Last());
            }

            if (RadioStationsContainer.RecentStations.Contains(station))
            {
                RadioStationsContainer.RecentStations.Remove(station);
            }

            RadioStationsContainer.RecentStations.Insert(0, station);

            await SaveRecentToFileAsync();
        }

        public static async Task ChangeIsFavoriteAsync(RadioStation station)
        {
            if (RadioStationsContainer.FavoriteStations.Contains(station))
            {
                RadioStationsContainer.FavoriteStations.Remove(station);
            }
            else
            {
                RadioStationsContainer.FavoriteStations.Add(station);
            }

            await SaveFavoriteToFileAsync();
        }

        private static async Task SaveAllStationsToFile()
        {
            await SaveAllToFileAsync();
            await SaveRecentToFileAsync();
            await SaveFavoriteToFileAsync();
            await SavePremiumToFileAsync();
        }

        private static async Task SaveAllToFileAsync() =>
            await folder.SaveAsync(Cfg.RadioStationsFileName, RadioStationsContainer.AllStations);
        private static async Task SaveRecentToFileAsync() =>
            await folder.SaveAsync(Cfg.RecentStationsFileName, RadioStationsContainer.RecentStations);
        private static async Task SaveFavoriteToFileAsync() =>
            await folder.SaveAsync(Cfg.FavoriteStationsFileName, RadioStationsContainer.FavoriteStations);
        private static async Task SavePremiumToFileAsync() =>
            await folder.SaveAsync(Cfg.PremiumStationsFileName, RadioStationsContainer.PremiumStations);

        private static async Task<ObservableCollection<RadioStation>> LoadAllFromFileAsync() =>
            await folder.ReadAsync<ObservableCollection<RadioStation>>(Cfg.RadioStationsFileName);
        private static async Task<ObservableCollection<RadioStation>> LoadRecentFromFileAsync() =>
            await folder.ReadAsync<ObservableCollection<RadioStation>>(Cfg.RecentStationsFileName);
        private static async Task<ObservableCollection<RadioStation>> LoadFavoriteFromFileAsync() =>
            await folder.ReadAsync<ObservableCollection<RadioStation>>(Cfg.FavoriteStationsFileName);
        private static async Task<ObservableCollection<RadioStation>> LoadPremiumFromFileAsync() =>
            await folder.ReadAsync<ObservableCollection<RadioStation>>(Cfg.PremiumStationsFileName);


        private static async Task SaveFilteredStationsFromApiToContainerAsync(Filter options) =>
            RadioStationsContainer.AllStations = FilterStations(await LoadStationsFromApiByAllCountryAsync(), options);
        private static async Task SavePremiumFromSomewhereToContainerASync() =>
            RadioStationsContainer.PremiumStations = await LoadPremiumStationsFromSomewhereAsync();

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

        private static async Task LoadAllFromFileToContainerAsync()
        {
            RadioStationsContainer.AllStations = await LoadAllFromFileAsync();
            RadioStationsContainer.PremiumStations = await LoadPremiumFromFileAsync();
            var RecentStations = await LoadRecentFromFileAsync();
            var FavoriteStations = await LoadFavoriteFromFileAsync();

            RadioStationsContainer.FavoriteStations = FavoriteStations ?? new ObservableCollection<RadioStation>();
            RadioStationsContainer.RecentStations = RecentStations ?? new ObservableCollection<RadioStation>();
        }
        private static async Task<ObservableCollection<RadioStation>> LoadStationsFromApiByAllCountryAsync()
        {
            var allStations = new List<RadioStation>();
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

        private static async Task<ObservableCollection<RadioStation>> LoadPremiumStationsFromSomewhereAsync()
        {
            // TODO: Change to API or save in file before release
            var premiumStation = new RadioStation
            {
                Name = "Anison",
                Url = "http://anison.fm/anison.m3u",
                Favicon = "https://anison.fm/images/main_maskot.png?v=4",
                Country = "Russia",
                Language = "English, Japan",
                CountryCode = "RU",
                HomePage = "https://anison.fm/",
                Tags = "Anime, Japan",
                Bitrate = 128
            };
            return new ObservableCollection<RadioStation> { premiumStation };
        }

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
    }
}
