using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Helpers;
using NRadio.Models;
using NRadio.Core.Services;
using NRadio.Services;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace NRadio.ViewModels
{
    public class StationDetailViewModel : ObservableObject
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ViewModelLocator vml;

        private RadioStation currentStation;
        private List<RadioStation> source = new List<RadioStation>();
        private int currentSongIndex;
        private string favoriteGlyph;

        public ICommand OpenPlayerCommand => new AsyncRelayCommand(OnOpenPlayer);
        public ICommand ChangeFavoriteStateCommand => new RelayCommand(OnChangeFavoriteState);
        public ICommand OpenRecordingPageCommand => new AsyncRelayCommand(OnOpenRecordingPage);

        public StationDetailViewModel(IServiceProvider serviceProvider)
        {
            System.Diagnostics.Debug.WriteLine("StationDetailViewModel created");
            this.serviceProvider = serviceProvider;
            vml = serviceProvider.GetService<ViewModelLocator>();
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
            var purchaseProvider = PurchaseService.PurchaseProvider;
            if (!IsPremiumStation(CurrentStation) || await purchaseProvider.CheckIfUserHasPremiumAsync())
            {
                await vml.PlayerVM.Initialize(Playlist, CurrentStationIndex);
                NavigationService.Navigate(NavigationTarget.Target.PlayerPage);
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
        
        private async Task OnOpenRecordingPage()
        {
            var purchaseProvider = PurchaseService.PurchaseProvider;
            if (!IsPremiumStation(CurrentStation) || await purchaseProvider.CheckIfUserHasPremiumAsync())
            {
                vml.RecordingVM.Initialize(CurrentStation);
                NavigationService.Navigate(NavigationTarget.Target.RecordingPage);
            }
            else
            {
                await DialogService.PremiumNotActiveDialogAsync();
            }
        }

        private void SetFavoriteGlyph() =>
            FavoriteGlyph = RadioStationsContainer.FavoriteStations.Contains(CurrentStation)
                ? ResourceLoader.GetForCurrentView("Resources").GetString("Favorite_Glyph")
                : ResourceLoader.GetForCurrentView("Resources").GetString("Not_Favorite_Glyph");

        private bool IsPremiumStation(RadioStation station) => RadioStationsContainer.PremiumStations.Contains(station);
    }
}
