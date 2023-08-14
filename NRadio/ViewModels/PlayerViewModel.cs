using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Core;

namespace NRadio.ViewModels
{
    public class PlayerViewModel : ObservableObject
    {
        const double DefaultVolume = 50;

        private int currentStationIndex;
        private RadioStation CurrentStation;
        private List<RadioStation> radioStations;
        private string stationName;
        private string stationUrl;
        private string stationDescription;
        private string stationImageUrl;
        private string playGlyph;
        private string favoriteGlyph;
        private double volume = DefaultVolume;
        private bool isPlaying;

        public PlayerViewModel()
        {
            System.Diagnostics.Debug.WriteLine("PlayerVM created");
        }

        public ICommand PlayPauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand PlayNextCommand { get; private set; }
        public ICommand PlayPreviousCommand { get; private set; }
        public ICommand UpdateAudioListCommand { get; private set; }
        public ICommand ChangeFavoriteStateCommand { get; private set; }

        public string StationName
        {
            get => stationName;
            set => SetProperty(ref stationName, value);
        }
        public string StationUrl
        {
            get => stationUrl;
            set
            {
                SetProperty(ref stationUrl, value);
                AddToRecent();
            }
        }
        public string StationDescription
        {
            get => stationDescription;
            set => SetProperty(ref stationDescription, value);
        }
        public string StationImageUrl
        {
            get => stationImageUrl;
            set => SetProperty(ref stationImageUrl, value);
        }
        public string PlayGlyph
        {
            get => playGlyph;
            set => SetProperty(ref playGlyph, value);
        }
        public string FavoriteGlyph
        {
            get => favoriteGlyph; 
            set => SetProperty(ref favoriteGlyph, value); 
        }
        public double Volume
        {
            get { return volume; }
            set
            {
                SetProperty(ref volume, value);
                SetVolume();
            }
        }
        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                SetProperty(ref isPlaying, value);
                SetPlayGlyphString();
            }
        }

        public void Initialize(List<RadioStation> radioStations, int index)
        {
            System.Diagnostics.Debug.WriteLine("PlayerVM initialized");

            PlayPauseCommand = new RelayCommand(PlayPause);
            StopCommand = new RelayCommand(Stop);
            PlayNextCommand = new RelayCommand(PlayNext);
            PlayPreviousCommand = new RelayCommand(PlayPrevious);
            UpdateAudioListCommand = new AsyncRelayCommand(UpdateAudioList);
            ChangeFavoriteStateCommand = new AsyncRelayCommand(ChangeFavoriteState);

            this.radioStations = radioStations;
            currentStationIndex = index;
            
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Volume"))
            {
                Volume = (double)ApplicationData.Current.LocalSettings.Values["Volume"];
            }

            SetCurrentStation();
            SetGlyphsFromResources();

            PlayerService.NextButtonPressed += async () =>
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, PlayNext);
            PlayerService.PreviousButtonPressed += async () =>
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, PlayPrevious);

            PlayerService.PlayRadioStream(StationUrl);
        
            IsPlaying = true;
        }

        public void ChangePlaylist(List<RadioStation> radioStations, int currentStationIndex)
        {
            this.radioStations = radioStations;
            this.currentStationIndex = currentStationIndex;

            if (StationName != CurrentStation.Name)
            {
                SetCurrentStation();
                PlayerService.PlayRadioStream(StationUrl);
            }
        }

        private void SetCurrentStation()
        {
            CurrentStation = radioStations[currentStationIndex];
            StationName = CurrentStation.Name;
            StationUrl = CurrentStation.Url;
            StationImageUrl = CurrentStation.Favicon;
        }

        private void PlayNext()
        {
            if (currentStationIndex < radioStations.Count - 1)
            {
                currentStationIndex++;
            }
            else
            {
                currentStationIndex = 0;
            }

            SetCurrentStation();
            SetFavoriteGlyph();

            if (IsPlaying)
            {
                PlayerService.PlayRadioStream(StationUrl);
            }
        }

        private void PlayPrevious()
        {
            if (currentStationIndex > 0)
            {
                currentStationIndex--;
            }
            else
            {
                currentStationIndex = radioStations.Count - 1;
            }

            SetCurrentStation();
            SetFavoriteGlyph();    

            if (IsPlaying)
            {
                PlayerService.PlayRadioStream(StationUrl);
            }
        }
     
        private void PlayPause()
        {
            if (!IsPlaying)
            {
                PlayerService.PlayRadioStream(StationUrl);
                IsPlaying = true;
            }
            else
            {
                PlayerService.PauseRadioStream();
                IsPlaying = false;
            }
        }

        private void Stop() => PlayerService.StopRadioStream();

        private void SetVolume() => PlayerService.SetVolume(Volume / 100); // Volume in PlayerService is in range 0-1

        private async Task UpdateAudioList() => await RadioStationsLoader.UpdateRadioStationsAsync();

        private void SetGlyphsFromResources()
        {
            PlayGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Play_Glyph");
            SetFavoriteGlyph();
        }

        private void SetFavoriteGlyph()
        {
            if(RadioStationsContainer.FavoriteStations.Contains(CurrentStation))
            {
                FavoriteGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Favorite_Glyph");
            }
            else
            {
                FavoriteGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Not_Favorite_Glyph");
            }
        }

        private void SetPlayGlyphString()
        {
            if (IsPlaying)
            {
                PlayGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Pause_Glyph");
            }
            else
            {
                PlayGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Play_Glyph");
            }
        }

        private async Task AddToRecent() => await RadioStationsLoader.AddToLastRecentAsync(CurrentStation);

        private async Task ChangeFavoriteState()
        {
            if (radioStations.Count == 0) 
            {
                var lastStation = RadioStationsContainer.RecentStations.Last();
                await RadioStationsLoader.ChangeIsFavoriteAsync(lastStation);
            }

            await RadioStationsLoader.ChangeIsFavoriteAsync(CurrentStation);
            SetFavoriteGlyph();
        }
    }
}
