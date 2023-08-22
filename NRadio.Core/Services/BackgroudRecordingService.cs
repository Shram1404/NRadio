using NRadio.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NRadio.Core.Services
{
    public static class BackgroundRecordingService
    {
        const int DelayInMillisecond = 5000;

        private static List<RecordingStation> recStations = new List<RecordingStation>();
        private static bool isListContainsStation = false;

        public static List<RecordingStation> RecStations
        {
            get => recStations;
            private set => recStations = value;
        }

        public static async Task StartSchedulerAsync()
        {
            RecStations = await RadioStationsLoader.LoadRecStationsAsync();
            if(RecStations == null)
            {
                RecStations = new List<RecordingStation>();
            }
            CheckIsListContainStationsAsync();
            Debug.WriteLine("BackgroundRecordingService: StartSchedulerAsync");
            while (true)
            {
                Debug.WriteLine("BackgroundRecordingService: StartSchedulerAsync: CheckForStartRecording" + DateTime.Now.TimeOfDay);
                await Task.Delay(DelayInMillisecond);
                await CheckForStartRecording();
            }
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

        private static void CheckIsListContainStationsAsync()
        {
            if (RecStations.Count == 0)
            {
                isListContainsStation = false;
            }
            else
            {
                isListContainsStation = true;
            }
        }

        private static async Task CheckForStartRecording()
        {
            if (isListContainsStation)
            {
                Debug.WriteLine("BackgroundRecordingService: CheckForStartRecording: List contains stations");
                try
                {
                    foreach (var s in RecStations)
                    {
                        if (DateTime.Now >= s.StartTime && DateTime.Now < s.EndTime)
                        {
                            var station = s;

                            await RemoveStationFromSchedulerListAsync(s);
                            Debug.WriteLine("BackgroundRecordingService: CheckForStartRecording: Start recording");
                            Task start = Task.Run(() => RadioRecorder.StartRecordingAsync(station.Uri, station.Name));
                            Task stopChecker = Task.Run(() => CheckForStopRecording(station));
                            await Task.WhenAll(start, stopChecker);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("BackgroundRecordingService: CheckForStartRecordingExc: " + ex.Message);
                }
            }
            CheckIsListContainStationsAsync();
        }

        private static async Task CheckForStopRecording(RecordingStation station)
        {
            try
            {
                while (DateTime.Now <= station.EndTime)
                {
                    Debug.WriteLine("BackgroundRecordingService: CheckForStopRecording" + DateTime.Now.TimeOfDay);
                    await Task.Delay(DelayInMillisecond);
                }
                await RadioRecorder.StopRecordingAsync(true);
                Debug.WriteLine("BackgroundRecordingService: CheckForStopRecording: Stop recording" + DateTime.Now.TimeOfDay);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CheckForStopRecording Exception: " + ex.Message);
            }
        }
    }
}
