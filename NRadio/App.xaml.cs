using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NRadio.Core.Activation;
using NRadio.Controls;
using NRadio.Core.Services;
using NRadio.Helpers;
using NRadio.Models;
using NRadio.Core.Services;
using NRadio.ViewModels;
using NRadio.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI.Xaml;

namespace NRadio
{
    public sealed partial class App : Application
    {
        private ServiceProvider serviceProvider;
        private readonly Lazy<ActivationService> activationService;

        public App()
        {
            InitializeComponent();

            RadioStationsLoader.InitializeAsync();

            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;
            UnhandledException += OnAppUnhandledException;

            activationService = new Lazy<ActivationService>(CreateActivationService);
            IdentityService.LoggedOut += OnLoggedOut;

            InitializeNavigationComponents();
            RegisterServices();
        }

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;
        private ActivationService ActivationService => activationService.Value;
        public ViewModelLocator ViewModelLocator => serviceProvider.GetService<ViewModelLocator>();

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            await RequestBackgroundRecordingAsync();
            await BackgroundWorkService.InitializeBackgroundTaskService();

            await RequestVoiceControlAsync();
        }

        protected override async void OnActivated(IActivatedEventArgs args) => await ActivationService.ActivateAsync(args);

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args) =>
            await ActivationService.ActivateAsync(args);

        private void RegisterServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ViewModelLocator>();

            services.AddTransient<BrowseViewModel>();
            services.AddTransient<HorizontalItemScrollViewModel>();
            services.AddTransient<LogInViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddSingleton<PlayerViewModel>();
            services.AddSingleton<RecordingViewModel>();
            services.AddTransient<SearchViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<ShellViewModel>();
            services.AddSingleton<StationDetailViewModel>();
            services.AddSingleton<StationsListViewModel>();
            services.AddTransient<UserViewModel>();

            services.AddTransient<UserDataService>();

            serviceProvider = services.BuildServiceProvider();
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Unhandled exception: " + e.Message);
            System.Diagnostics.Debug.WriteLine(e.Exception.StackTrace);
        }

        private ActivationService CreateActivationService() =>
            new ActivationService(this, NavigationTarget.Target.MainPage, new Lazy<UIElement>(CreateShell));

        private UIElement CreateShell() => new Views.ShellPage();

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        { }

        private async void OnLoggedOut(object sender, EventArgs e)
        {
            ActivationService.SetShell(new Lazy<UIElement>(CreateShell));
            await ActivationService.RedirectLoginPageAsync();
        }

        private async Task RequestBackgroundRecordingAsync()
        {
            var newSession = new ExtendedExecutionSession
            {
                Reason = ExtendedExecutionReason.Unspecified,
                Description = "Long Running Radio Recorder"
            };
            newSession.Revoked += Session_Revoked;
            var result = await newSession.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    _ = Task.Run(async () => await BackgroundRecordingService.StartSchedulerAsync());
                    break;

                default:
                case ExtendedExecutionResult.Denied:
                    break;
            }
        }

        private async void Session_Revoked(object sender, ExtendedExecutionRevokedEventArgs args) =>
            await BackgroundRecordingService.StopSchedulerAsync();

        private async Task RequestVoiceControlAsync()
        {
            await AudioCapturePermissions.RequestMicrophonePermissionAsync();

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("VoiceControlAllowed") && (bool)localSettings.Values["VoiceControlAllowed"])
            {
                var voiceControl = new VoiceControlService();

                try
                {
                    await voiceControl.InitializeAsync();
                    _ = Task.Run(async () => await voiceControl.StartListeningAsync());
                }
                catch (InvalidOperationException)
                {
                    voiceControl.Dispose();
                }
            }
        }

        private void InitializeNavigationComponents()
        {
            InitializeNavigationService();

        }
        private void InitializeNavigationService()
        {
            var pages = new Dictionary<Type, NavigationTarget.Target>
            {
                { typeof(BrowsePage), NavigationTarget.Target.BrowsePage },
                { typeof(MainPage), NavigationTarget.Target.MainPage },
                { typeof(PlayerPage), NavigationTarget.Target.PlayerPage },
                { typeof(RecordingPage), NavigationTarget.Target.RecordingPage },
                { typeof(SearchPage), NavigationTarget.Target.SearchPage },
                { typeof(SettingsPage), NavigationTarget.Target.SettingsPage },
                { typeof(StationDetailPage), NavigationTarget.Target.StationDetailPage },
                { typeof(StationsListPage), NavigationTarget.Target.StationsListPage },
                { typeof(LogInPage), NavigationTarget.Target.LogInPage },
                { typeof(FirstRunDialogPage), NavigationTarget.Target.FirstRunDialog }
            };

            NavigationService.Initialize(pages);
        }
    }
}