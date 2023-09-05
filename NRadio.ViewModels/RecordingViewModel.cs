using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Services;
using NRadio.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NRadio.ViewModels
{
    public class RecordingViewModel : ObservableObject
    {
        private readonly IServiceProvider serviceProvider;
        private RadioStation currentStation;
        private TimeSpan startTime = DateTime.Now.TimeOfDay;
        private TimeSpan endTime = DateTime.Now.TimeOfDay;

        public RecordingViewModel(IServiceProvider serviceProvider)
        {
            System.Diagnostics.Debug.WriteLine("RecordingViewModel created");
            this.serviceProvider = serviceProvider;
        }

        public ICommand AddToScheduler { get; private set; }

        public RadioStation CurrentStation
        {
            get => currentStation;
            set => SetProperty(ref currentStation, value);
        }
        public TimeSpan StartTime
        {
            get => startTime;
            set => SetProperty(ref startTime, value);
        }
        public TimeSpan EndTime
        {
            get => endTime;
            set => SetProperty(ref endTime, value);
        }

        public void Initialize(RadioStation station)
        {
            CurrentStation = station;
            AddToScheduler = new AsyncRelayCommand(OnAddToScheduler);
        }

        private async Task OnAddToScheduler()
        {
            var startDate = DateTime.Today + StartTime;
            var endDate = DateTime.Today + EndTime;

            if (StartTime >= EndTime && startDate < DateTime.Now)
            {
                // TODO: Show error message
            }
            else
            {
                var recordingStation = new RecordingStation
                {
                    Name = CurrentStation.Name,
                    Uri = CurrentStation.Url,
                    StartTime = startDate,
                    EndTime = endDate
                };
                await BackgroundRecordingService.AddStationToSchedulerListAsync(recordingStation);
            }
        }
    }
}