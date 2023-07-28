using System;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace NRadio.Core.Services
{
    public static class PlayerService
    {
        private static SystemMediaTransportControls _systemMediaControls;
        private static MediaPlayer _mediaPlayer = new MediaPlayer();
        private static string _currentUrl;

        public static event Action NextButtonPressed;
        public static event Action PreviousButtonPressed;

        static PlayerService()
        {
            _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
            _systemMediaControls.ButtonPressed += SystemMediaControls_ButtonPressed;
            _mediaPlayer.CommandManager.IsEnabled = false;
        }

        public static void PlayRadioStream(string url)
        {
            SetStation(url);
            _mediaPlayer.Play();

            _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Playing;
            EnableSystemMediaControls();

        }
        public static void PauseRadioStream()
        {
            _mediaPlayer.Pause();
            _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Paused;
        }
        public static void StopRadioStream()
        {
            _mediaPlayer.Pause();
            _mediaPlayer.Dispose();
            _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
        }
        public static void SetStation(string url)
        {
            if (url != _currentUrl || _mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.None)
            {
                _currentUrl = url;
                var mediaSource = MediaSource.CreateFromUri(new Uri(url));
                _mediaPlayer.Source = mediaSource;
                _mediaPlayer.Play();
            }
        }
        public static void SetVolume(double volume) => _mediaPlayer.Volume = volume;


        private static void EnableSystemMediaControls()
        {
            _systemMediaControls.IsPlayEnabled = true;
            _systemMediaControls.IsPauseEnabled = true;
            _systemMediaControls.IsStopEnabled = true;
            _systemMediaControls.IsNextEnabled = true;
            _systemMediaControls.IsPreviousEnabled = true;
        }
        private static void SystemMediaControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    PlayRadioStream(_currentUrl);
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    PauseRadioStream();
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    StopRadioStream();
                    break;
                case SystemMediaTransportControlsButton.Next:
                    NextButtonPressed?.Invoke();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    PreviousButtonPressed?.Invoke();
                    break;
                default:
                    break;
            }
        }
    }
}
