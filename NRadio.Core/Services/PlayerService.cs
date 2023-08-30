using System;
using Windows.Devices.Enumeration;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace NRadio.Core.Services
{
    public static class PlayerService
    {
        private static SystemMediaTransportControls systemMediaControls;
        private static MediaPlayer mediaPlayer = new MediaPlayer();
        private static string currentUrl;

        static PlayerService()
        {
            mediaPlayer.RealTimePlayback = true;

            systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
            systemMediaControls.ButtonPressed += SystemMediaControls_ButtonPressed;
            mediaPlayer.CommandManager.IsEnabled = false;
        }

        public static event Action NextButtonPressed;
        public static event Action PreviousButtonPressed;

        public static void PlayRadioStream(string url)
        {
            if (mediaPlayer is null || mediaPlayer.PlaybackSession is null)
            {
                mediaPlayer = new MediaPlayer();
            }
            if (currentUrl is null || currentUrl != url)
            {
                SetStation(url);
            }
            mediaPlayer.Play();

            systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Playing;
            EnableSystemMediaControls();
        }
        public static void PauseRadioStream()
        {
            mediaPlayer.Pause();
            systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Paused;
        }
        public static void StopRadioStream()
        {
            mediaPlayer.Pause();
            mediaPlayer.Dispose();
            systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
        }

        public static void SetStation(string url)
        {
            if (url != currentUrl || mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.None)
            {
                currentUrl = url;
                var mediaSource = MediaSource.CreateFromUri(new Uri(url));
                mediaPlayer.Source = mediaSource;
            }
        }

        public static void SetVolume(double volume) => mediaPlayer.Volume = volume;

        public static void SetDeviceForMediaPlayer(DeviceInformation device) => mediaPlayer.AudioDevice = device;

        private static void EnableSystemMediaControls()
        {
            systemMediaControls.IsPlayEnabled = true;
            systemMediaControls.IsPauseEnabled = true;
            systemMediaControls.IsStopEnabled = true;
            systemMediaControls.IsNextEnabled = true;
            systemMediaControls.IsPreviousEnabled = true;
        }

        private static void SystemMediaControls_ButtonPressed(SystemMediaTransportControls sender,
                                                              SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    PlayRadioStream(currentUrl);
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
