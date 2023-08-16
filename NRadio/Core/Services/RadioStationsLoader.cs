﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using NRadio.Core.API;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Helpers;
using NRadio.Services;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace NRadio.Core.Services
{
    public static class RadioStationsLoader
    {
        private const int MaxRecentStations = 20;

        private static readonly StorageFolder folder = ApplicationData.Current.LocalFolder;
        private static readonly Filter stationFilter = new Filter
        {
            HasName = true,
            HasUrl = true,
            HasTag = false,
            HasFavicon = true,
            HasCountry = true,
            HasLanguage = true,
            MinBitrate = 64
        };
        private static bool isUpdating = false;

        private static Config cfg { get; set; }

        public static async Task InitializeAsync()
        {
            var configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///config.json"));
            string configJson = await FileIO.ReadTextAsync(configFile);
            cfg = JsonSerializer.Deserialize<Config>(configJson);

            await CheckFilesState();
        }

        public static async Task UpdateRadioStationsAsync()
        {
            if (!isUpdating)
            {
                isUpdating = true;
                await SaveFilteredStationsFromApiToContainerAsync(stationFilter);
                await SavePremiumFromSomewhereToContainerASync();
                await SaveAllStationsToFile();
                await LoadAllFromFileToContainerAsync();
                isUpdating = false;

                Debug.WriteLine("RadioStations was updated");
            }
            else
            {
                Debug.WriteLine("RadioStations is already updating");
            }
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

        public static async Task ShowUpdateStationsMessageAsync() => await DialogService.NeedStationsUpdateDialogAsync();

        private static async Task CheckFilesState()
        {
            bool isUpdated = false;
            try
            {
                await LoadAllFromFileToContainerAsync();
            }
            catch (FileNotFoundException)
            {
                if (!SystemInformation.Instance.IsFirstRun)
                {
                    await ShowUpdateStationsMessageAsync();
                }

                await UpdateRadioStationsAsync();
                isUpdated = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (!isUpdated && (RadioStationsContainer.AllStations is null || RadioStationsContainer.AllStations.Count == 0))
            {
                if (!SystemInformation.Instance.IsFirstRun)
                {
                    await ShowUpdateStationsMessageAsync();
                }

                await UpdateRadioStationsAsync();
            }
        }

        private static async Task SaveAllStationsToFile()
        {
            await SaveAllToFileAsync();
            await SaveRecentToFileAsync();
            await SaveFavoriteToFileAsync();
            await SavePremiumToFileAsync();
        }

        private static async Task SaveAllToFileAsync() =>
            await folder.SaveAsync(cfg.RadioStationsFileName, RadioStationsContainer.AllStations);
        private static async Task SaveRecentToFileAsync() =>
            await folder.SaveAsync(cfg.RecentStationsFileName, RadioStationsContainer.RecentStations);
        private static async Task SaveFavoriteToFileAsync() =>
            await folder.SaveAsync(cfg.FavoriteStationsFileName, RadioStationsContainer.FavoriteStations);
        private static async Task SavePremiumToFileAsync() =>
            await folder.SaveAsync(cfg.PremiumStationsFileName, RadioStationsContainer.PremiumStations);

        private static async Task<List<RadioStation>> LoadAllFromFileAsync() =>
            await folder.ReadAsync<List<RadioStation>>(cfg.RadioStationsFileName);
        private static async Task<List<RadioStation>> LoadRecentFromFileAsync() =>
            await folder.ReadAsync<List<RadioStation>>(cfg.RecentStationsFileName);
        private static async Task<List<RadioStation>> LoadFavoriteFromFileAsync() =>
            await folder.ReadAsync<List<RadioStation>>(cfg.FavoriteStationsFileName);
        private static async Task<List<RadioStation>> LoadPremiumFromFileAsync() =>
            await folder.ReadAsync<List<RadioStation>>(cfg.PremiumStationsFileName);

        private static async Task SaveFilteredStationsFromApiToContainerAsync(Filter options) =>
            RadioStationsContainer.AllStations = FilterStations(await LoadStationsFromApiByAllCountryAsync(), options);
        private static async Task SavePremiumFromSomewhereToContainerASync() =>
            RadioStationsContainer.PremiumStations = await LoadPremiumStationsFromSomewhereAsync();

        private static List<RadioStation> FilterStations(List<RadioStation> stations, Filter filter)
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

            return new List<RadioStation>(filteredStations);
        }

        private static async Task LoadAllFromFileToContainerAsync()
        {
            RadioStationsContainer.AllStations = await LoadAllFromFileAsync();
            RadioStationsContainer.PremiumStations = await LoadPremiumFromFileAsync();
            var RecentStations = await LoadRecentFromFileAsync();
            var FavoriteStations = await LoadFavoriteFromFileAsync();

            RadioStationsContainer.FavoriteStations = FavoriteStations ?? new List<RadioStation>();
            RadioStationsContainer.RecentStations = RecentStations ?? new List<RadioStation>();
        }
        private static async Task<List<RadioStation>> LoadStationsFromApiByAllCountryAsync()
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

            return new List<RadioStation>(allStations);
        }

        private static async Task<List<RadioStation>> LoadPremiumStationsFromSomewhereAsync()
        {
            // TODO: Change to API before release
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
            return new List<RadioStation> { premiumStation };
        }

        private enum Countries // TODO: Add all countries or change to file or config storage before release
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
