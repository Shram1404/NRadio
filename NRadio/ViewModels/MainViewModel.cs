﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
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
        public ICommand ScrollLeftRecentCommand { get; private set; }
        public ICommand ScrollRightRecentCommand { get; private set; }
        public ICommand ScrollLeftFavoriteCommand { get; private set; }
        public ICommand ScrollRightFavoriteCommand { get; private set; }
        public ICommand ScrollLeftLocalCommand { get; private set; }
        public ICommand ScrollRightLocalCommand { get; private set; }
        public ICommand ItemClickCommand { get; private set; }

        private int moveOffset = 200;

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

            ScrollLeftFavoriteCommand = new RelayCommand(() => FavoriteHorizontalOffset -= moveOffset);
            ScrollRightFavoriteCommand = new RelayCommand(() => FavoriteHorizontalOffset += moveOffset);
            ScrollLeftRecentCommand = new RelayCommand(() => RecentHorizontalOffset -= moveOffset);
            ScrollRightRecentCommand = new RelayCommand(() => RecentHorizontalOffset += moveOffset);
            ScrollLeftLocalCommand = new RelayCommand(() => LocalHorizontalOffset -= moveOffset);
            ScrollRightLocalCommand = new RelayCommand(() => LocalHorizontalOffset += moveOffset);
            ItemClickCommand = new RelayCommand<RadioStation>(OnOpenStationDetail);

            Initialize();
        }

        public void Initialize()
        {
            Debug.WriteLine("MainViewModel initialized");

            Recent = RadioStationsContainer.RecentStations;
            Favorite = RadioStationsContainer.FavoriteStations;
            Local = new ObservableCollection<RadioStation>(RadioStationsContainer.AllStations.Where(s => s.CountryCode == "UA")); // TODO: Change to current locale
        }

        private void OnOpenStationDetail(RadioStation clickedItem)
        {
            ObservableCollection<RadioStation> Source = null;
            if (Favorite.Contains(clickedItem)) // TODO: Change to normal check
                Source = Favorite;
            else if (Recent.Contains(clickedItem))
                Source = Recent;
            else if (Local.Contains(clickedItem))
                Source = Local;
            else
                throw new Exception("Unknown station");

            NavigationService.Navigate<StationDetailPage>(clickedItem.Name);
            ((App)Application.Current).ViewModelLocator.StationDetailVM.CurrentSongIndex = Source.IndexOf(clickedItem);
            ((App)Application.Current).ViewModelLocator.StationDetailVM.Source = Source;
        }
    }
}
