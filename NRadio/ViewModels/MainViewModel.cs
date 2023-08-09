using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Services;
using NRadio.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

        private ObservableCollection<RadioStation> recentStations;
        private ObservableCollection<RadioStation> favoriteStations;
        private ObservableCollection<RadioStation> localStations;

        public MainViewModel()
        {
            Debug.WriteLine("MainViewModel created");

            ScrollLeftFavoriteCommand = new RelayCommand(() => FavoriteHorizontalOffset -= MoveOffset);
            ScrollRightFavoriteCommand = new RelayCommand(() => FavoriteHorizontalOffset += MoveOffset);
            ScrollLeftRecentCommand = new RelayCommand(() => RecentHorizontalOffset -= MoveOffset);
            ScrollRightRecentCommand = new RelayCommand(() => RecentHorizontalOffset += MoveOffset);
            ScrollLeftLocalCommand = new RelayCommand(() => LocalHorizontalOffset -= MoveOffset);
            ScrollRightLocalCommand = new RelayCommand(() => LocalHorizontalOffset += MoveOffset);
            ItemClickCommand = new RelayCommand<RadioStation>(OnOpenStationDetail);

            Initialize();
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

        public ObservableCollection<RadioStation> Recent
        {
            get => recentStations;
            set => SetProperty(ref recentStations, value);
        }
        public ObservableCollection<RadioStation> Favorite
        {
            get => favoriteStations;
            set => SetProperty(ref favoriteStations, value);
        }
        public ObservableCollection<RadioStation> Local
        {
            get => localStations;
            set => SetProperty(ref localStations, value);
        }

        public void Initialize()
        {
            Debug.WriteLine("MainViewModel initialized");

            Recent = RadioStationsContainer.RecentStations;
            Favorite = RadioStationsContainer.FavoriteStations;
            Local = new ObservableCollection<RadioStation>(RadioStationsContainer.AllStations.Where(s => s.CountryCode == "UA")); // TODO: Change to real locale
        }

        private void OnOpenStationDetail(RadioStation clickedItem)
        {
            ObservableCollection<RadioStation> Source = null;
            if (Favorite.Contains(clickedItem)) // TODO: Change to normal check
            {
                Source = Favorite;
            }
            else if (Recent.Contains(clickedItem))
            {
                Source = Recent;
            }
            else if (Local.Contains(clickedItem))
            {
                Source = Local;
            }
            else
            {
                throw new ArgumentException("Unknown station");
            }

            NavigationService.Navigate<StationDetailPage>(clickedItem.Name);
            ((App)Application.Current).ViewModelLocator.StationDetailVM.CurrentSongIndex = Source.IndexOf(clickedItem);
            ((App)Application.Current).ViewModelLocator.StationDetailVM.Source = Source;
        }
    }
}