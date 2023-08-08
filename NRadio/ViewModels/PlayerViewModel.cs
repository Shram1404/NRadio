using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace NRadio.ViewModels
{
    public class PlayerViewModel : ObservableObject
    {

        public ICommand PlayPauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand PlayNextCommand { get; private set; }
        public ICommand PlayPreviousCommand { get; private set; }
        public ICommand UpdateAudioListCommand { get; private set; }
        public ICommand ChangeFavoriteStateCommand { get; private set; }

        private ObservableCollection<RadioStation> _radioStations;
        private int _currentSongIndex;

        private string _stationName;
        public string StationName
        {
            get => _stationName;
            set => SetProperty(ref _stationName, value);
        }
        private string _stationUrl;
        public string StationUrl
        {
            get => _stationUrl;
            set
            {
                SetProperty(ref _stationUrl, value);
                AddToRecent();
            }
        }

        private string _stationDescription;
        public string StationDescription
        {
            get => _stationDescription;
            set => SetProperty(ref _stationDescription, value);
        }
        private string _stationImageUrl;
        public string StationImageUrl
        {
            get => _stationImageUrl;
            set => SetProperty(ref _stationImageUrl, value);
        }
        private double _volume = 50;
        public double Volume
        {
            get { return _volume; }
            set
            {
                SetProperty(ref _volume, value);
                OnValueChanged();
            }
        }

        private string _playGlyph = "\uE768"; // Play glyph
        public string PlayGlyph
        {
            get => _playGlyph;
            set => SetProperty(ref _playGlyph, value);
        }
        private string _favoriteGlyph;
        public string FavoriteGlyph
        {
            get { return _favoriteGlyph; }
            set { SetProperty(ref _favoriteGlyph, value); }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                SetProperty(ref _isPlaying, value);
                IsPlayGlyphString();
            }
        }

        public Slider VolumeSlider { get; set; }

        public PlayerViewModel()
        {
            System.Diagnostics.Debug.WriteLine("PlayerVM created");
        }

        public void Initialize(ObservableCollection<RadioStation> radioStations, int index)
        {
            System.Diagnostics.Debug.WriteLine("PlayerVM initialized");
            _radioStations = radioStations;
            _currentSongIndex = index;

            PlayPauseCommand = new RelayCommand(PlayPause);
            StopCommand = new RelayCommand(Stop);
            PlayNextCommand = new RelayCommand(PlayNext);
            PlayPreviousCommand = new RelayCommand(PlayPrevious);
            UpdateAudioListCommand = new RelayCommand(UpdateAudioList);
            ChangeFavoriteStateCommand = new RelayCommand(ChangeFavoriteState);

            RadioStation Item = _radioStations[_currentSongIndex];
            FavoriteGlyph = RadioStationsContainer.FavoriteStations.Contains(Item) ? "\xE735" : "\xE734";

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Volume"))
            {
                Volume = (double)ApplicationData.Current.LocalSettings.Values["Volume"];
            }

            SetStationForPage();

            PlayerService.NextButtonPressed += async () => await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, PlayNext);
            PlayerService.PreviousButtonPressed += async () => await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, PlayPrevious);

            PlayPause();
            IsPlaying = true;
        }

        private void PlayNext()
        {
            if (_currentSongIndex < _radioStations.Count - 1)
                _currentSongIndex++;
            else
                _currentSongIndex = 0;

            SetStationForPage();
            if (IsPlaying)
                PlayerService.SetStation(StationUrl);
        }
        private void PlayPrevious()
        {
            if (_currentSongIndex > 0)
                _currentSongIndex--;
            else
                _currentSongIndex = _radioStations.Count - 1;

            SetStationForPage();
            if (IsPlaying)
                PlayerService.SetStation(StationUrl);
        }
        private void SetStationForPage()
        {
            var currentSong = _radioStations[_currentSongIndex];
            StationName = currentSong.Name;
            StationUrl = currentSong.Url;
            StationImageUrl = currentSong.Favicon;
        }

        public void PlayPause()
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

        private void OnValueChanged() => SetVolume();
        private void SetVolume()
        {
            if (Volume == 0)
                PlayerService.SetVolume(0);
            else if (Volume == 100)
                PlayerService.SetVolume(1);
            else
                PlayerService.SetVolume(Volume / 100);
        }

        private void IsPlayGlyphString()
        {
            if (IsPlaying) PlayGlyph = "\uE769"; // Pause glyph
            else PlayGlyph = "\uE768"; // Play glyph
        }

        public void ChangePlaylist(ObservableCollection<RadioStation> radioStations, int currentSongIndex)
        {
            _radioStations = radioStations;
            _currentSongIndex = currentSongIndex;

            if (StationName != _radioStations[_currentSongIndex].Name)
            {
                SetStationForPage();
                PlayerService.PlayRadioStream(StationUrl);
            }

        }
        private async void UpdateAudioList() => await RadioStationsLoader.UpdateRadioStations();

        private async Task AddToRecent()
        {
            var currentSong = _radioStations[_currentSongIndex];
            await RadioStationsLoader.AddToLastRecentAsync(currentSong);
        }

        private async void ChangeFavoriteState()
        {
            if(_radioStations.Count == 0)
            {
                int LastStationIndex = RadioStationsContainer.RecentStations.Count - 1;
                RadioStation LastStation = RadioStationsContainer.RecentStations[LastStationIndex];
                RadioStationsLoader.ChangeIsFavoriteAsync(LastStation);
            }
            RadioStation Item = _radioStations[_currentSongIndex];
            await RadioStationsLoader.ChangeIsFavoriteAsync(Item);
            FavoriteGlyph = RadioStationsContainer.FavoriteStations.Contains(Item) ? "\xE735" : "\xE734";
        }
    }
}
