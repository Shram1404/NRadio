using NRadio.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace NRadio.Core.Services
{
    public sealed class VoiceControlService : IDisposable
    {
        const int NotFoundHResult = -2147023728;

        private SpeechRecognizer recognizer;
        private double lastVolume;

        public bool VoiceControlAllowed { get; set; }

        public async Task InitializeAsync()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("VoiceControlAllowed"))
            {
                VoiceControlAllowed = (bool)localSettings.Values["VoiceControlAllowed"];
            }

            try
            {
                recognizer = new SpeechRecognizer();
                recognizer.Constraints.Add(new SpeechRecognitionListConstraint(new List<string>() { "play", "stop", "next", "previous", "mute", "unmute" }));
            }
            catch (Exception exception)
            {
                if (exception.HResult == NotFoundHResult)
                {
                    await DialogService.SpeechComponentErrorDialogAsync();
                    throw new InvalidOperationException("Speech component not found", exception);
                }
                else
                {
                    throw new InvalidOperationException("Could not initialize SpeechRecognizer", exception);
                }
            }
        }

        public async Task StartListeningAsync()
        {
            while (VoiceControlAllowed)
            {
                string command = await ListenAsync();
                if (command == "radio")
                {
                    Console.Beep(4000, 100);
                    await DoCommandAsync(await ListenAsync());
                }
            }
        }

        public void Dispose()
        {
            recognizer?.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task<string> ListenAsync()
        {
            var result = await recognizer.RecognizeAsync();
            if (result.Status == SpeechRecognitionResultStatus.Success)
            {
                return result.Text;
            }

            return null;
        }

        private async Task DoCommandAsync(string command)
        {
            var playerVM = ViewModelLocatorHelper.GetViewModelInstance(Models.VMLocatorEnum.VM.PlayerVM);
            
            lastVolume = playerVM.Volume;
            switch (command)
            {
                case "play":
                    playerVM.PlayPause();
                    break;
                case "stop":
                    playerVM.PlayPause();
                    break;
                case "next":
                    playerVM.PlayNext();
                    break;
                case "previous":
                    playerVM.PlayPrevious();
                    break;
                case "mute":
                    playerVM.Volume = 0;
                    break;
                case "unmute":
                    playerVM.Volume = lastVolume;
                    break;
                default:
                    break;
            }
            await StartListeningAsync();
        }
    }
}
