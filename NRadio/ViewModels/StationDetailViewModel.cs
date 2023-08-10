using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;
using NRadio.Services;
using NRadio.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
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

        public ICommand OpenPlayerCommand => new RelayCommand(OnOpenPlayer);
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

        private void OnOpenPlayer()
        {
            ((App)Application.Current).ViewModelLocator.PlayerVM.Initialize(Playlist, CurrentStationIndex);
            NavigationService.Navigate<PlayerPage>();
        }
        private async void OnChangeFavoriteState()
        {
            await RadioStationsLoader.ChangeIsFavoriteAsync(CurrentStation);
            SetFavoriteGlyph();
        }
        private void SetFavoriteGlyph()
        {
            if (RadioStationsContainer.FavoriteStations.Contains(CurrentStation))
            {
                FavoriteGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Favorite_Glyph");
            }
            else
            {
                FavoriteGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Not_Favorite_Glyph");
            }
        }
    }
}
