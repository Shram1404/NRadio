using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;

namespace NRadio.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private const int MaxStations = 20;

        private List<RadioStation> recentStations;
        private List<RadioStation> favoriteStations;
        private List<RadioStation> localStations;

        public MainViewModel()
        {
            Debug.WriteLine("MainViewModel created");

            InitializeAsync();
        }

        public List<RadioStation> Recent
        {
            get => recentStations;
            set => SetProperty(ref recentStations, value);
        }
        public List<RadioStation> Favorite
        {
            get => favoriteStations;
            set => SetProperty(ref favoriteStations, value);
        }
        public List<RadioStation> Local
        {
            get => localStations;
            set => SetProperty(ref localStations, value);
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("MainViewModel initialized");

            try
            {
                Recent = RadioStationsContainer.RecentStations.Take(MaxStations).ToList();
                Favorite = RadioStationsContainer.FavoriteStations.Take(MaxStations).ToList();
                Local = new List<RadioStation>(RadioStationsContainer.AllStations.Where(s => s.CountryCode == "UA").Take(MaxStations)); // TODO: Change to real locale
            }
            catch (NullReferenceException)
            {
                await Task.Run(async () =>
                {
                    await RadioStationsLoader.ShowUpdateStationsMessageAsync();
                    await RadioStationsLoader.InitializeAsync();
                });
            }
        }
    }
}