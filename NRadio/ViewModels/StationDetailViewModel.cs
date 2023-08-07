﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;
using NRadio.Services;
using NRadio.Views;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    public class StationDetailViewModel : ObservableObject
    {
        public ICommand OpenPlayerCommand => new RelayCommand(OnOpenPlayer);
        public ICommand ChangeFavoriteStateCommand => new RelayCommand(OnChangeFavoriteState);

        private RadioStation _item;
        public RadioStation Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        private ObservableCollection<RadioStation> _source = new ObservableCollection<RadioStation>();
        public ObservableCollection<RadioStation> Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        private int _currentSongIndex;
        public int CurrentSongIndex
        {
            get { return _currentSongIndex; }
            set { SetProperty(ref _currentSongIndex, value); }
        }

        private string _favoriteGlyph;
        public string FavoriteGlyph
        {
            get { return _favoriteGlyph; }
            set { SetProperty(ref _favoriteGlyph, value); }
        }

        public StationDetailViewModel()
        {
            System.Diagnostics.Debug.WriteLine("StationDetailViewModel created");
        }

        public void Initialize(string name)
        {
            var data = RadioStationsContainer.AllStations; // TODO: change to current playlist
            Item = data.FirstOrDefault(i => i.Name == name);
            FavoriteGlyph = RadioStationsContainer.FavoriteStations.Contains(Item) ? "\xE735" : "\xE734";
        }

        private void OnOpenPlayer()
        {
            ((App)Application.Current).ViewModelLocator.PlayerVM.Initialize(Source, CurrentSongIndex);
            NavigationService.Navigate<PlayerPage>();
        }
        private async void OnChangeFavoriteState()
        {
            await RadioStationsLoader.ChangeIsFavoriteAsync(Item);
            FavoriteGlyph = RadioStationsContainer.FavoriteStations.Contains(Item) ? "\xE735" : "\xE734";
        }
    }
}
