using Newtonsoft.Json;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

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

            RadioStationsContainer.AllStations = await LoadRadioStationsFromFileAsync();
            RadioStationsContainer.RecentsStations = await LoadRecentStationsFromFileAsync();
        }

        public static async Task UpdateRadiostationsAsync()
        {
            await GetAndSaveRadioStationsFromServerAsync();
            RadioStationsContainer.AllStations = await LoadRadioStationsFromFileAsync();
        }

        private static async Task GetAndSaveRadioStationsFromServerAsync() => await SaveRadioStationsToFileAsync(await GetStationsFromServerAsync());

        private static async Task<ObservableCollection<RadioStation>> GetStationsFromServerAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Cfg.ServerUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var radioStations = JsonConvert.DeserializeObject<ObservableCollection<RadioStation>>(json);
                    return radioStations;
                }
            }
            return new ObservableCollection<RadioStation>();
        }

        private static async Task SaveRadioStationsToFileAsync(ObservableCollection<RadioStation> radioStations)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(Cfg.RadioStationsFileName, CreationCollisionOption.ReplaceExisting);
            string jsonData = JsonConvert.SerializeObject(radioStations, Formatting.Indented);
            await FileIO.WriteTextAsync(file, jsonData);
        }

        private static async Task<ObservableCollection<RadioStation>> LoadRadioStationsFromFileAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await localFolder.GetFileAsync(Cfg.RadioStationsFileName);
                string jsonData = await FileIO.ReadTextAsync(file);
                var radioStations = JsonConvert.DeserializeObject<ObservableCollection<RadioStation>>(jsonData);
                return radioStations;
            }
            catch (FileNotFoundException)
            {
                await GetAndSaveRadioStationsFromServerAsync();
                return await LoadRadioStationsFromFileAsync();
            }
        }

        public static async Task AddToLast20RecentsAsync(RadioStation station)
        {
            if (RadioStationsContainer.RecentsStations.Count >= 20)
                RadioStationsContainer.RecentsStations.RemoveAt(0);

            RadioStationsContainer.RecentsStations.Add(station);
            await SaveRecentStationsToFileAsync(RadioStationsContainer.RecentsStations);
        }
        private static async Task SaveRecentStationsToFileAsync(ObservableCollection<RadioStation> recentStations)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(Cfg.RecentStationsFileName, CreationCollisionOption.ReplaceExisting);
            string jsonData = JsonConvert.SerializeObject(recentStations, Formatting.Indented);
            await FileIO.WriteTextAsync(file, jsonData);
        }
        private static async Task<ObservableCollection<RadioStation>> LoadRecentStationsFromFileAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await localFolder.GetFileAsync(Cfg.RecentStationsFileName);
                string jsonData = await FileIO.ReadTextAsync(file);
                var recentStations = JsonConvert.DeserializeObject<ObservableCollection<RadioStation>>(jsonData);
                return recentStations;
            }
            catch (FileNotFoundException)
            {
                return new ObservableCollection<RadioStation>();
            }
        }

    }
}
