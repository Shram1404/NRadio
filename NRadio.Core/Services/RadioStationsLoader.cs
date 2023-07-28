using Newtonsoft.Json;
using NRadio.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using NRadio.Core.Helpers;
using System.Threading.Tasks;

namespace NRadio.Core.Services
{
    public static class RadioStationsLoader
    {
        public static ConfigService Cfg { get; private set; }

        public static async Task Initialize()
        {
            StorageFile configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///config.json"));
            string configJson = await FileIO.ReadTextAsync(configFile);
            Cfg = JsonConvert.DeserializeObject<ConfigService>(configJson);

            RadioStationsContainer.AllStations = await LoadRadioStationsFromFile();
            RadioStationsContainer.RecentsStations = await LoadRecentStationsFromFile();
        }

        public static async Task UpdateRadiostations()
        {
            await GetAndSaveRadioStationsFromServer();
            RadioStationsContainer.AllStations = await LoadRadioStationsFromFile();
        }

        private static async Task GetAndSaveRadioStationsFromServer() => await SaveRadioStationsToFile(await GetStationsFromServer());

        private static async Task<List<RadioStation>> GetStationsFromServer()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Cfg.ServerUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var radioStations = JsonConvert.DeserializeObject<List<RadioStation>>(json);
                    return radioStations;
                }
            }
            return new List<RadioStation>();
        }

        private static async Task SaveRadioStationsToFile(List<RadioStation> radioStations)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(Cfg.RadioStationsFileName, CreationCollisionOption.ReplaceExisting);
            string jsonData = JsonConvert.SerializeObject(radioStations, Formatting.Indented);
            await FileIO.WriteTextAsync(file, jsonData);
        }

        private static async Task<List<RadioStation>> LoadRadioStationsFromFile()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await localFolder.GetFileAsync(Cfg.RadioStationsFileName);
                string jsonData = await FileIO.ReadTextAsync(file);
                var radioStations = JsonConvert.DeserializeObject<List<RadioStation>>(jsonData);
                return radioStations;
            }
            catch (FileNotFoundException)
            {
                await GetAndSaveRadioStationsFromServer();
                return await LoadRadioStationsFromFile();
            }
        }

        public static async Task AddToLast20Recents(RadioStation station)
        {
            if (RadioStationsContainer.RecentsStations.Count >= 20)
                RadioStationsContainer.RecentsStations.RemoveAt(0);

            RadioStationsContainer.RecentsStations.Add(station);
            await SaveRecentStationsToFile(RadioStationsContainer.RecentsStations);
        }
        private static async Task SaveRecentStationsToFile(List<RadioStation> recentStations)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(Cfg.RecentStationsFileName, CreationCollisionOption.ReplaceExisting);
            string jsonData = JsonConvert.SerializeObject(recentStations, Formatting.Indented);
            await FileIO.WriteTextAsync(file, jsonData);
        }
        private static async Task<List<RadioStation>> LoadRecentStationsFromFile()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await localFolder.GetFileAsync(Cfg.RecentStationsFileName);
                string jsonData = await FileIO.ReadTextAsync(file);
                var recentStations = JsonConvert.DeserializeObject<List<RadioStation>>(jsonData);
                return recentStations;
            }
            catch (FileNotFoundException)
            {
                return new List<RadioStation>();
            }
        }

    }
}
