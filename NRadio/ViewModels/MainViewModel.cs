using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using NRadio.Helpers;
using NRadio.Models;
using NRadio.Core.Services;

namespace NRadio.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private const int MaxStations = 20;

        private List<RadioStation> recentStations;
        private List<RadioStation> favoriteStations;
        private List<RadioStation> localStations;
        private bool recentVisible;
        private bool favoriteVisible;
        private bool localVisible;

        public MainViewModel()
        {
            Debug.WriteLine("MainViewModel created");
            InitializeAsync();
        }

        public List<RadioStation> Recent
        {
            get => recentStations;
            set
            {
                SetProperty(ref recentStations, value);
                CheckVisibility();
            }
        }
        public List<RadioStation> Favorite
        {
            get => favoriteStations;
            set
            {
                SetProperty(ref favoriteStations, value);
                CheckVisibility();
            }
        }
        public List<RadioStation> Local
        {
            get => localStations;
            set
            {
                SetProperty(ref localStations, value);
                CheckVisibility();
            }
        }
        public bool RecentVisible
        {
            get => recentVisible;
            set => SetProperty(ref recentVisible, value);
        }
        public bool FavoriteVisible
        {
            get => favoriteVisible;
            set => SetProperty(ref favoriteVisible, value);
        }
        public bool LocalVisible
        {
            get => localVisible;
            set => SetProperty(ref localVisible, value);
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("MainViewModel initialized");

            try
            {
                Recent = RadioStationsContainer.RecentStations.Take(MaxStations).ToList();
                Favorite = RadioStationsContainer.FavoriteStations.Take(MaxStations).ToList();
                Local = new List<RadioStation>(RadioStationsContainer.AllStations.Where(s =>
                    s.CountryCode.ToUpper() == LocationService.GetCountryCode()).Take(MaxStations).ToList());

            }
            catch (NullReferenceException)
            {
                await Task.Run(async () =>
                {
                    await RadioStationsLoader.ShowUpdateStationsMessageAsync();
                    await RadioStationsLoader.InitializeAsync();
                });
            }

            CheckVisibility();
        }

        private void CheckVisibility()
        {
            RecentVisible = Recent != null && Recent.Count > 0;
            FavoriteVisible = Favorite != null && Favorite.Count > 0;
            LocalVisible = Local != null && Local.Count > 0;
        }
    }
}