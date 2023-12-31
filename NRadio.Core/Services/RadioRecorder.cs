﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace NRadio.Core.Services
{
    public class RadioRecorder
    {
        private StorageFile bufferFile;
        private CancellationTokenSource cts;

        /// <summary>
        /// This static static method is used to toggle recording and save media file. url is source, fileName is the name of the file to be saved, 
        /// isPathDefault is used to determine if the default path should be used or if the user should be prompted to choose a path.
        /// </summary>
        public async Task ToggleRecordingAsync(string url, string fileName, bool isPathDefault)
        {
            if (IsStationRecording(fileName))
            {
                await StopRecordingAsync(isPathDefault);
            }
            else
            {
                await StartRecordingAsync(url, fileName);
            }
        }

        public void Cancel() => cts.Cancel();

        public async Task StartRecordingAsync(string url, string fileName)
        {
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(url);
            var file = await CreateRecordingFileAsync(fileName);
            if (file != null)
            {
                var output = await file.OpenAsync(FileAccessMode.ReadWrite);
                bufferFile = file;
                cts = new CancellationTokenSource();
                try
                {
                    await stream.CopyToAsync(output.AsStreamForWrite(), 81920, cts.Token)
                        .ContinueWith(t =>
                        {
                            stream.Dispose();
                            output.Dispose();
                            client.Dispose();
                        });
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Recording cancelled");
                }
            }
        }

        public async Task StopRecordingAsync(bool isPathDefault)
        {
            var endTime = DateTimeOffset.Now;
            var newFileName = $"{bufferFile.DisplayName}_{endTime:(HH-mm-ss)}";

            cts.Cancel();

            StorageFile transcodedFile;
            if (isPathDefault)
            {
                transcodedFile = await CreateTranscodedFileInMusicLibraryAsync(newFileName);
            }
            else
            {
                transcodedFile = await CreateTranscodedFileAsync(newFileName);
            }
            await TranscodeToMp3Async(transcodedFile);
        }

        public bool IsStationRecording(string name) => bufferFile != null && bufferFile.DisplayName.Contains(name);

        private async Task<StorageFile> CreateRecordingFileAsync(string fileName)
        {
            DateTimeOffset startTime = DateTimeOffset.Now;
            fileName = $"{fileName}_{startTime:MM-dd(HH-mm-ss)}";
            var file = await CreateTranscodedFileInMusicLibraryAsync(fileName);
            return file;
        }
        private async Task<StorageFile> CreateTranscodedFileAsync(string fileName)
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.MusicLibrary,
                SuggestedFileName = fileName
            };
            savePicker.FileTypeChoices.Add("MP3 Audio", new[] { ".mp3" });

            StorageFile file = await savePicker.PickSaveFileAsync() ?? await CreateTranscodedFileInMusicLibraryAsync(fileName);
            return file;
        }

        private async Task<StorageFile> CreateTranscodedFileInMusicLibraryAsync(string fileName)
        {
            fileName = $"{fileName}.mp3";
            var musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            var folder = musicLibrary.SaveFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            return file;
        }

        private async Task TranscodeToMp3Async(StorageFile outputFile)
        {
            var transcoder = new MediaTranscoder();
            var profile = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High);
            using (var bufferStream = await GetStreamFromFileAsync())
            {
                using (var transcodedStream = new InMemoryRandomAccessStream())
                {
                    var preparedTranscodeResult = await transcoder.PrepareStreamTranscodeAsync(bufferStream, transcodedStream, profile);

                    if (preparedTranscodeResult.CanTranscode)
                    {
                        cts = new CancellationTokenSource();
                        try
                        {
                            var progress = new Progress<double>(DelBufferOnTranscodingCompleteAsync);
                            await preparedTranscodeResult.TranscodeAsync().AsTask(cts.Token, progress);

                            await SaveStreamToFileAsync(transcodedStream, outputFile);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to transcode: {ex.Message}");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Transcoding to mp3 failed: {preparedTranscodeResult.FailureReason}");
                    }
                }
            }
        }

        private async void DelBufferOnTranscodingCompleteAsync(double percent)
        {
            Debug.WriteLine($"Transcoding progress:  {percent.ToString().Split('.')[0]}%");

            byte transcodingIsComplete = 100;
            if (percent == transcodingIsComplete)
            {
                cts.Cancel();
                cts.Dispose();
                try
                {
                    await bufferFile.DeleteAsync();
                    Debug.WriteLine($"Buffer file {bufferFile.DisplayName} was deleted");
                    bufferFile = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to delete {bufferFile.DisplayName}: {ex.Message} {bufferFile.Path}");
                }
            }
        }

        private async Task<IRandomAccessStream> GetStreamFromFileAsync()
        {
            IRandomAccessStream readStream = await bufferFile.OpenAsync(FileAccessMode.Read);
            return readStream;
        }
        private async Task SaveStreamToFileAsync(IRandomAccessStream transcodedStream, StorageFile outputFile)
        {
            using (var outputStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var dataWriter = new DataWriter(outputStream);
                transcodedStream.Seek(0);
                var buffer = new Windows.Storage.Streams.Buffer((uint)transcodedStream.Size);
                await transcodedStream.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.None);
                dataWriter.WriteBuffer(buffer);
                await dataWriter.StoreAsync();
                await outputStream.FlushAsync();
            }
        }
    }
}