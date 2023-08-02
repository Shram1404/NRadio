using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using System.Linq;
using System.Diagnostics;

namespace NRadio.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ICommand ScrollLeftRecentCommand { get; private set; }
        public ICommand ScrollRightRecentCommand { get; private set; }
        public ICommand ScrollLeftFavoriteCommand { get; private set; }
        public ICommand ScrollRightFavoriteCommand { get; private set; }
        public ICommand ScrollLeftLocalCommand { get; private set; }
        public ICommand ScrollRightLocalCommand { get; private set; }

        private double _recentHorizontalOffset;
        public double RecentHorizontalOffset
        {
            get => _recentHorizontalOffset;
            set
            {
                value = Math.Max(0, value);
                SetProperty(ref _recentHorizontalOffset, value);
            }
        }
        private double _favoriteHorizontalOffset;
        public double FavoriteHorizontalOffset
        {
            get => _favoriteHorizontalOffset;
            set
            {
                value = Math.Max(0, value);
                SetProperty(ref _favoriteHorizontalOffset, value);
            }
        }
        private double _localHorizontalOffset;
        public double LocalHorizontalOffset
        {
            get => _localHorizontalOffset;
            set
            {
                value = Math.Max(0, value);
                SetProperty(ref _localHorizontalOffset, value);
            }
        }

        private ObservableCollection<RadioStation> _recentStations;
        public ObservableCollection<RadioStation> Recent
        {
            get => _recentStations;
            set => SetProperty(ref _recentStations, value);
        }
        private ObservableCollection<RadioStation> _favoriteStations;
        public ObservableCollection<RadioStation> Favorite
        {
            get => _favoriteStations;
            set => SetProperty(ref _favoriteStations, value);
        }
        private ObservableCollection<RadioStation> _localStations;
        public ObservableCollection<RadioStation> Local
        {
            get => _localStations;
            set => SetProperty(ref _localStations, value);
        }

        public MainViewModel()
        {
            Debug.WriteLine("MainViewModel created");

            ScrollLeftFavoriteCommand = new RelayCommand(() => FavoriteHorizontalOffset -= 200);
            ScrollRightFavoriteCommand = new RelayCommand(() => FavoriteHorizontalOffset += 200);
            ScrollLeftRecentCommand = new RelayCommand(() => RecentHorizontalOffset -= 200);
            ScrollRightRecentCommand = new RelayCommand(() => RecentHorizontalOffset += 200);
            ScrollLeftLocalCommand = new RelayCommand(() => LocalHorizontalOffset -= 200);
            ScrollRightLocalCommand = new RelayCommand(() => LocalHorizontalOffset += 200);

            Recent = RadioStationsContainer.RecentsStations;
            Favorite = new ObservableCollection<RadioStation>(RadioStationsContainer.AllStations.Where(s => s.IsFavorite));
            Local = new ObservableCollection<RadioStation>(RadioStationsContainer.AllStations.Where(s => s.CountryCode == "UA")); // TODO: Change to current locale
        }
    }
}
