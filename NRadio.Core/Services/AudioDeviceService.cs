using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;

namespace NRadio.Core.Services
{
    public class AudioDeviceService
    {
        private DeviceWatcher deviceWatcher;

        public event EventHandler AudioDevicesChanged;

        public string DefaultAudioDevice { get; set; }

        public async Task<List<DeviceInformation>> GetAudioDevicesAsync()
        {
            var deviceSelector = MediaDevice.GetAudioRenderSelector();
            var allDevices = await DeviceInformation.FindAllAsync(deviceSelector);
            return allDevices.Where(d => d.IsEnabled).ToList();
        }

        public async Task<DeviceInformation> GetDefaultDeviceAsync()
        {
            var deviceSelector = MediaDevice.GetAudioRenderSelector();
            var allDevices = await DeviceInformation.FindAllAsync(deviceSelector);
            var defaultDeviceId = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            return allDevices.FirstOrDefault(d => d.Id == defaultDeviceId);
        }

        public void StartWatchingAudioDevices()
        {
            var deviceSelector = MediaDevice.GetAudioRenderSelector();
            deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);
            deviceWatcher.Added += OnAudioDeviceAdded;
            deviceWatcher.Removed += OnAudioDeviceRemoved;
            deviceWatcher.Updated += OnAudioDeviceUpdated;
            deviceWatcher.Start();
        }

        private void OnAudioDevicesChanged() => AudioDevicesChanged?.Invoke(this, EventArgs.Empty);

        private void OnAudioDeviceAdded(DeviceWatcher sender, DeviceInformation args) => OnAudioDevicesChanged();
        private void OnAudioDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args) => OnAudioDevicesChanged();
        private void OnAudioDeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate args) => OnAudioDevicesChanged();
    }
}
