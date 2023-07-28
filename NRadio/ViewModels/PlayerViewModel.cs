using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Models;
using NRadio.Core.Services;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace NRadio.ViewModels
{
    public class PlayerViewModel : ObservableObject
    {

        public PlayerViewModel()
        {
            System.Diagnostics.Debug.WriteLine("PlayerViewModel createsd");
        }
        public RelayCommand PlayPauseCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand PlayNextCommand { get; private set; }
        public RelayCommand PlayPreviousCommand { get; private set; }
        public RelayCommand UpdateAudioListCommand { get; private set; }

        private List<RadioStation> _radioStations;
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
            set => SetProperty(ref _stationUrl, value);
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

        private string _isPlayingString = "Play";
        public string IsPlayingString
        {
            get => _isPlayingString;
            set => SetProperty(ref _isPlayingString, value);
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                SetProperty(ref _isPlaying, value);
                IsPlayString();
            }
        }

        public Slider VolumeSlider { get; set; }

        public PlayerViewModel(List<RadioStation> radioStations, int index)
        {
            System.Diagnostics.Debug.WriteLine("PlayerViewModel created");

            _radioStations = radioStations;
            _currentSongIndex = index;

            PlayPauseCommand = new RelayCommand(PlayPause);
            StopCommand = new RelayCommand(Stop);
            PlayNextCommand = new RelayCommand(PlayNext);
            PlayPreviousCommand = new RelayCommand(PlayPrevious);
            UpdateAudioListCommand = new RelayCommand(UpdateAudioList);

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Volume"))
            {
                Volume = (double)ApplicationData.Current.LocalSettings.Values["Volume"];
            }

            SetStationForPage();

            PlayerService.NextButtonPressed += async () => await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, PlayNext);
            PlayerService.PreviousButtonPressed += async () => await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, PlayPrevious);

            IsPlaying = false;
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

            RadioStationsLoader.AddToLast20Recents(currentSong);
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

        private void IsPlayString()
        {
            if (IsPlaying) IsPlayingString = "Pause";
            else IsPlayingString = "Play";
        }

        public void ChangePlaylist(List<RadioStation> radioStations, int currentSongIndex)
        {
            _radioStations = radioStations;
            _currentSongIndex = currentSongIndex;

            if (StationName != _radioStations[_currentSongIndex].Name)
            {
                SetStationForPage();
                PlayerService.PlayRadioStream(StationUrl);
            }

        }
        private async void UpdateAudioList() => await RadioStationsLoader.UpdateRadiostations();
    }
}
