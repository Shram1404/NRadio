using System;
using System.Threading.Tasks;
using NRadio.Activation;
using NRadio.Helpers;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace NRadio.Services
{
    internal class SuspendAndResumeService : ActivationHandler<LaunchActivatedEventArgs>
    {
        private const string StateFilename = "SuspendAndResumeState";

        public event EventHandler<SuspendAndResumeArgs> OnBackgroundEntering;
        public event EventHandler<SuspendAndResumeArgs> OnDataRestored;
        public event EventHandler OnResuming;

        public async Task<bool> SaveStateAsync()
        {
            if (OnBackgroundEntering == null)
            {
                return false;
            }

            try
            {
                var suspensionState = new SuspensionState()
                {
                    SuspensionDate = DateTime.Now
                };

                var target = OnBackgroundEntering?.Target.GetType();
                var onBackgroundEnteringArgs = new SuspendAndResumeArgs(suspensionState, target);

                OnBackgroundEntering?.Invoke(this, onBackgroundEnteringArgs);

                await ApplicationData.Current.LocalFolder.SaveAsync(StateFilename, onBackgroundEnteringArgs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ResumeApp()
        {
            OnResuming?.Invoke(this, EventArgs.Empty);
        }

        public async Task RestoreSuspendAndResumeData()
        {
            var saveState = await GetSuspendAndResumeData();
            if (saveState != null)
            {
                OnDataRestored?.Invoke(this, saveState);
            }
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            var saveState = await GetSuspendAndResumeData();
            if (saveState?.Target != null && typeof(Page).IsAssignableFrom(saveState.Target))
            {
                NavigationService.Navigate(saveState.Target, saveState.SuspensionState);
            }
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            return args.PreviousExecutionState == ApplicationExecutionState.Terminated;
        }

        public async Task<SuspendAndResumeArgs> GetSuspendAndResumeData()
        {
            var saveState = await ApplicationData.Current.LocalFolder.ReadAsync<SuspendAndResumeArgs>(StateFilename);
            if (saveState?.Target != null && typeof(Page).IsAssignableFrom(saveState.Target))
            {
                return saveState;
            }

            return null;
        }
    }
}
