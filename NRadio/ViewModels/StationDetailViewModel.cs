﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;
using NRadio.Core.Services.Purchase;
using NRadio.Services;
using NRadio.Views;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    public class StationDetailViewModel : ObservableObject
    {
        private RadioStation currentStation;
        private List<RadioStation> source = new List<RadioStation>();
        private int currentSongIndex;
        private string favoriteGlyph;

        public ICommand OpenPlayerCommand => new AsyncRelayCommand(OnOpenPlayer);
        public ICommand ChangeFavoriteStateCommand => new RelayCommand(OnChangeFavoriteState);

        public StationDetailViewModel()
        {
            System.Diagnostics.Debug.WriteLine("StationDetailViewModel created");
        }

        public RadioStation CurrentStation
        {
            get => currentStation;
            set => SetProperty(ref currentStation, value);
        }
        public List<RadioStation> Playlist
        {
            get => source;
            set => SetProperty(ref source, value);
        }
        public int CurrentStationIndex
        {
            get => currentSongIndex;
            set => SetProperty(ref currentSongIndex, value);
        }
        public string FavoriteGlyph
        {
            get => favoriteGlyph;
            set => SetProperty(ref favoriteGlyph, value);
        }

        public void Initialize(List<RadioStation> playlist, RadioStation currentStation, int currentStationIndex)
        {
            Playlist = playlist;
            CurrentStation = currentStation;
            CurrentStationIndex = currentStationIndex;
            SetFavoriteGlyph();
        }

        private async Task OnOpenPlayer()
        {
            IPurchaseProvider purchaseProvider = new PurchaseSimulatorProvider(); // TODO: Change to StoreContextProvider for release
            if (!IsPremiumStation(CurrentStation) || await purchaseProvider.CheckIfUserHasPremiumAsync())
            {
                ((App)Application.Current).ViewModelLocator.PlayerVM.Initialize(Playlist, CurrentStationIndex);
                NavigationService.Navigate<PlayerPage>();
            }
            else
            {
                await DialogService.PremiumNotActiveDialogAsync();
            }
        }

        private async void OnChangeFavoriteState()
        {
            await RadioStationsLoader.ChangeIsFavoriteAsync(CurrentStation);
            SetFavoriteGlyph();
        }

        private void SetFavoriteGlyph() =>
            FavoriteGlyph = RadioStationsContainer.FavoriteStations.Contains(CurrentStation)
                ? ResourceLoader.GetForCurrentView("Resources").GetString("Favorite_Glyph")
                : ResourceLoader.GetForCurrentView("Resources").GetString("Not_Favorite_Glyph");

        private bool IsPremiumStation(RadioStation station) => RadioStationsContainer.PremiumStations.Contains(station);
    }
}
