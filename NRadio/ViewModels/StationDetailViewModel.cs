using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;
using NRadio.Services;
using NRadio.Views;
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
        private ObservableCollection<RadioStation> source = new ObservableCollection<RadioStation>();
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
        public ObservableCollection<RadioStation> Source
        {
            get => source; 
            set => SetProperty(ref source, value); 
        }
        public int CurrentSongIndex
        {
            get => currentSongIndex;
            set => SetProperty(ref currentSongIndex, value);
        }
        public string FavoriteGlyph
        {
            get => favoriteGlyph; 
            set => SetProperty(ref favoriteGlyph, value); 
        }

        public void Initialize(string name)
        {
            var data = RadioStationsContainer.AllStations; // TODO: change to current playlist for faster loading
            CurrentStation = data.FirstOrDefault(i => i.Name == name);
            SetFavoriteGlyph();
        }

        private void OnOpenPlayer()
        {
            ((App)Application.Current).ViewModelLocator.PlayerVM.Initialize(Source, CurrentSongIndex);
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
