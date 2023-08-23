using NRadio.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NRadio.Core.Services
{
    public static class BackgroundRecordingService
    {
        const int DelayForCheck = 50000;
        const int DelayAfterRecording = 200;

        private static List<RecordingStation> recStations = new List<RecordingStation>();
        private static Dictionary<RecordingStation, RadioRecorder> recorderDict = new Dictionary<RecordingStation, RadioRecorder>();

        public static List<RadioRecorder> Recorders { get; private set; } = new List<RadioRecorder>();
        public static List<RecordingStation> RecStations
        {
            get => recStations;
            private set => recStations = value;
        }

        public static async Task StartSchedulerAsync()
        {
            await RemoveExpiredStationsFromFileAsync();
            RecStations = await RadioStationsLoader.LoadRecStationsAsync() ?? new List<RecordingStation>();

            Debug.WriteLine("BackgroundRecordingService: StartSchedulerAsync");
            while (true)
            {
                await Task.Delay(DelayForCheck);
                Task.Run(() => StartRecordingScheduledStationsAsync());
            }
        }

        public static async Task StopSchedulerAsync()
        {
            Debug.WriteLine("BackgroundRecordingService: StopSchedulerAsync");
            foreach (var recorder in Recorders)
            {
                await recorder.StopRecordingAsync(true);
            }
            Recorders.Clear();
        }

        public static async Task AddStationToSchedulerListAsync(RecordingStation station)
        {
            await RadioStationsLoader.AddRecStationToFileASync(station);
            RecStations.Add(station);
        }
        public static async Task RemoveStationFromSchedulerListAsync(RecordingStation station)
        {
            await RadioStationsLoader.RemoveRecStationFromFileASync(station);
            RecStations.Remove(station);
        }

        private static async Task StartRecordingScheduledStationsAsync()
        {
            Debug.WriteLine($"BackgroundRecordingService: StartRecordingScheduledStationsAsync check  {DateTime.Now.TimeOfDay}");
            if (RecStations.Count > 0)
            {
                await Task.Delay(DelayAfterRecording);
                try
                {
                    foreach (var s in RecStations)
                    {
                        if (DateTime.Now >= s.StartTime && DateTime.Now < s.EndTime)
                        {
                            var station = s;
                            await RemoveStationFromSchedulerListAsync(s);
                            RecStations = await RadioStationsLoader.LoadRecStationsAsync();

                            var recorder = GetOrCreateRecorderForStation(station);
                            Recorders.Add(recorder);

                            Debug.WriteLine("BackgroundRecordingService: StartRecordingScheduledStationsAsync: Start recording");
                            Task start = Task.Run(() => recorder.StartRecordingAsync(station.Uri, station.Name));
                            Task stopChecker = Task.Run(() => StopRecordingStationAsync(station));
                            await Task.WhenAll(start, stopChecker);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"BackgroundRecordingService: CheckForStartRecordingExc: {ex.Message}");
                }
            }
        }

        private static async Task StopRecordingStationAsync(RecordingStation station)
        {
            try
            {
                while (DateTime.Now <= station.EndTime)
                {
                    Debug.WriteLine($"BackgroundRecordingService: StopRecordingStationAsync check {station.Name} {DateTime.Now.TimeOfDay}");
                    await Task.Delay(DelayForCheck);
                }

                var recorder = recorderDict[station];
                await recorder.StopRecordingAsync(true);
                Debug.WriteLine($"BackgroundRecordingService: Stop recording {station.Name} {DateTime.Now.TimeOfDay}");

                Recorders.Remove(recorder);
                await RadioStationsLoader.RemoveRecStationFromFileASync(station);
                Debug.WriteLine($"BackgroundRecordingService: StopRecordingStationAsync: Remove station from scheduler file {DateTime.Now.TimeOfDay}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"StopRecordingStationAsync Exception: {ex.Message}");
            }
        }

        private static async Task RemoveExpiredStationsFromFileAsync()
        {
            var stationsList = await RadioStationsLoader.LoadRecStationsAsync();
            if (stationsList != null)
            {
                foreach (var s in stationsList)
                {
                    if (s.EndTime < DateTime.Now)
                    {
                        await RadioStationsLoader.RemoveRecStationFromFileASync(s);
                    }
                }
            }
        }

        private static RadioRecorder GetOrCreateRecorderForStation(RecordingStation s)
        {
            RadioRecorder recorder;
            if (recorderDict.ContainsKey(s))
            {
                recorder = recorderDict[s];
            }
            else
            {
                recorder = new RadioRecorder();
                recorderDict.Add(s, recorder);
            }
            return recorder;
        }
    }
}