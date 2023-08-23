﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Models;
using NRadio.Core.Services;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Core;

namespace NRadio.ViewModels
{
    public class PlayerViewModel : ObservableObject
    {
        const double DefaultVolume = 50;

        private bool isPlayerCreated = false;

        private int currentStationIndex;
        private RadioStation currentStation;
        private List<RadioStation> radioStations;
        private string stationName;
        private string stationUrl;
        private string stationDescription;
        private string stationImageUrl;
        private string playGlyph;
        private string favoriteGlyph;
        private string recordingGlyph;
        private double volume = DefaultVolume;
        private bool isPlaying;
        private RadioRecorder recorder;
        Dictionary<RadioStation, RadioRecorder> recorderDict = new Dictionary<RadioStation, RadioRecorder>();

        public PlayerViewModel()
        {
            System.Diagnostics.Debug.WriteLine("PlayerVM created");
        }

        public event EventHandler IsPlayerCreatedChanged;

        public ICommand PlayPauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand PlayNextCommand { get; private set; }
        public ICommand PlayPreviousCommand { get; private set; }
        public ICommand ToggleRecordingCommand { get; private set; }
        public ICommand ChangeFavoriteStateCommand { get; private set; }

        public bool IsPlayerCreated
        {
            get => isPlayerCreated;
            set
            {
                if (isPlayerCreated != value)
                {
                    isPlayerCreated = value;
                    IsPlayerCreatedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
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
        public string RecordingGlyph
        {
            get => recordingGlyph;
            set => SetProperty(ref recordingGlyph, value);
        }
        public double Volume
        {
            get => volume;
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
                SetPlayGlyph();
            }
        }

        public void Initialize(List<RadioStation> radioStations, int index)
        {
            System.Diagnostics.Debug.WriteLine("PlayerVM initialized");

            PlayPauseCommand = new RelayCommand(PlayPause);
            StopCommand = new RelayCommand(Stop);
            PlayNextCommand = new RelayCommand(PlayNext);
            PlayPreviousCommand = new RelayCommand(PlayPrevious);
            ToggleRecordingCommand = new AsyncRelayCommand(ToggleRecording);
            ChangeFavoriteStateCommand = new AsyncRelayCommand(ChangeFavoriteState);

            this.radioStations = radioStations;
            currentStationIndex = index;
            recorder = new RadioRecorder();

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
            IsPlayerCreated = true;
        }

        public void ChangePlaylist(List<RadioStation> radioStations, int currentStationIndex)
        {
            this.radioStations = radioStations;
            this.currentStationIndex = currentStationIndex;

            if (StationName != currentStation.Name)
            {
                SetCurrentStation();
                PlayerService.PlayRadioStream(StationUrl);
            }
        }

        private void SetCurrentStation()
        {
            currentStation = radioStations[currentStationIndex];
            StationName = currentStation.Name;
            StationUrl = currentStation.Url;
            StationImageUrl = currentStation.Favicon;
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

        private async Task ToggleRecording()
        {
            if (recorderDict.ContainsKey(currentStation))
            {
                recorder = recorderDict[currentStation];
            }
            else
            {
                recorder = new RadioRecorder();
                recorderDict.Add(currentStation, recorder);
            }

            SwapRecordingGlyph();
            await recorder.ToggleRecordingAsync(StationUrl, StationName, false);
        }

        private void SetGlyphsFromResources()
        {
            SetPlayGlyph();
            SetFavoriteGlyph();
            SetRecordingGlyph();
        }

        private void SwapRecordingGlyph()
        {
            if (RecordingGlyph == ResourceLoader.GetForCurrentView("Resources").GetString("Recording_Glyph"))
            {
                RecordingGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Not_Recording_Glyph");
            }
            else
            {
                RecordingGlyph = ResourceLoader.GetForCurrentView("Resources").GetString("Recording_Glyph");
            }
        }

        private void SetFavoriteGlyph() => FavoriteGlyph = RadioStationsContainer.FavoriteStations.Contains(currentStation)
                ? ResourceLoader.GetForCurrentView("Resources").GetString("Favorite_Glyph")
                : ResourceLoader.GetForCurrentView("Resources").GetString("Not_Favorite_Glyph");

        private void SetPlayGlyph() => PlayGlyph = IsPlaying
                ? ResourceLoader.GetForCurrentView("Resources").GetString("Pause_Glyph")
                : ResourceLoader.GetForCurrentView("Resources").GetString("Play_Glyph");

        private void SetRecordingGlyph()
        {
            RecordingGlyph = recorder.IsStationRecording(currentStation.Name)
                ? ResourceLoader.GetForCurrentView("Resources").GetString("Recording_Glyph")
                : ResourceLoader.GetForCurrentView("Resources").GetString("Not_Recording_Glyph");
        }

        private async Task AddToRecent() => await RadioStationsLoader.AddToLastRecentAsync(currentStation);

        private async Task ChangeFavoriteState()
        {
            if (radioStations.Count == 0)
            {
                var lastStation = RadioStationsContainer.RecentStations.Last();
                await RadioStationsLoader.ChangeIsFavoriteAsync(lastStation);
            }

            await RadioStationsLoader.ChangeIsFavoriteAsync(currentStation);
            SetFavoriteGlyph();
        }
    }
}
