using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;
using NRadio.Services;
using NRadio.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private const int MoveOffset = 200;

        private double recentHorizontalOffset;
        private double favoriteHorizontalOffset;
        private double localHorizontalOffset;

        private List<RadioStation> recentStations;
        private List<RadioStation> favoriteStations;
        private List<RadioStation> localStations;

        public MainViewModel()
        {
            Debug.WriteLine("MainViewModel created");

            ScrollLeftFavoriteCommand = new RelayCommand(() => FavoriteHorizontalOffset -= MoveOffset);
            ScrollRightFavoriteCommand = new RelayCommand(() => FavoriteHorizontalOffset += MoveOffset);
            ScrollLeftRecentCommand = new RelayCommand(() => RecentHorizontalOffset -= MoveOffset);
            ScrollRightRecentCommand = new RelayCommand(() => RecentHorizontalOffset += MoveOffset);
            ScrollLeftLocalCommand = new RelayCommand(() => LocalHorizontalOffset -= MoveOffset);
            ScrollRightLocalCommand = new RelayCommand(() => LocalHorizontalOffset += MoveOffset);
            ItemClickCommand = new RelayCommand<(RadioStation station, List<RadioStation> stations)>(OnOpenStationDetail);

            InitializeAsync();
        }

        public ICommand ScrollLeftRecentCommand { get; private set; }
        public ICommand ScrollRightRecentCommand { get; private set; }
        public ICommand ScrollLeftFavoriteCommand { get; private set; }
        public ICommand ScrollRightFavoriteCommand { get; private set; }
        public ICommand ScrollLeftLocalCommand { get; private set; }
        public ICommand ScrollRightLocalCommand { get; private set; }
        public ICommand ItemClickCommand { get; private set; }

        public double RecentHorizontalOffset
        {
            get => recentHorizontalOffset;
            set
            {
                value = Math.Max(0, value);
                SetProperty(ref recentHorizontalOffset, value);
            }
        }
        public double FavoriteHorizontalOffset
        {
            get => favoriteHorizontalOffset;
            set
            {
                value = Math.Max(0, value);
                SetProperty(ref favoriteHorizontalOffset, value);
            }
        }
        public double LocalHorizontalOffset
        {
            get => localHorizontalOffset;
            set
            {
                value = Math.Max(0, value);
                SetProperty(ref localHorizontalOffset, value);
            }
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
                Recent = RadioStationsContainer.RecentStations;
                Favorite = RadioStationsContainer.FavoriteStations;
                Local = new List<RadioStation>(RadioStationsContainer.AllStations.Where(s => s.CountryCode == "UA")); // TODO: Change to real locale
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

        private void OnOpenStationDetail((RadioStation clickedItem, List<RadioStation> source) args)
        {
            var (clickedItem, source) = args;
            NavigationService.Navigate<StationDetailPage>(clickedItem.Name);
            ((App)Application.Current).ViewModelLocator.StationDetailVM.Initialize(source, clickedItem, source.IndexOf(clickedItem));
        }
    }
}