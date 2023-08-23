using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NRadio.Core.Helpers;
using NRadio.Core.Services;
using NRadio.Purchase;
using NRadio.Services;
using NRadio.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using Windows.UI.Xaml;

namespace NRadio
{
    public sealed partial class App : Application
    {
        // TODO: Change to StoreContextProvider when app wil be in Dev Center
        public readonly IPurchaseProvider purchaseProvider = new PurchaseSimulatorProvider();

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
            await RequestBackgroundRecording();
            var applicationTrigger = new ApplicationTrigger();
            await applicationTrigger.RequestAsync();

            var backgroundTaskService = new BackgroundTaskService();
            await backgroundTaskService.RegisterBackgroundTasksAsync();
        }

        protected override async void OnActivated(IActivatedEventArgs args) => await ActivationService.ActivateAsync(args);

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void RegisterServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ViewModelLocator>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<ShellViewModel>();
            services.AddSingleton<RecordingViewModel>();
            services.AddSingleton<PlayerViewModel>();
            services.AddSingleton<StationDetailViewModel>();
            services.AddSingleton<StationsListViewModel>();

            serviceProvider = services.BuildServiceProvider();
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Unhandled exception: " + e.Message);
            System.Diagnostics.Debug.WriteLine(e.Exception.StackTrace);
        }

        private ActivationService CreateActivationService() =>
            new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));

        private UIElement CreateShell() => new Views.ShellPage();

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            Singleton<SuspendAndResumeService>.Instance.ResumeApp();
        }

        private async void OnLoggedOut(object sender, EventArgs e)
        {
            ActivationService.SetShell(new Lazy<UIElement>(CreateShell));
            await ActivationService.RedirectLoginPageAsync();
        }

        private async Task RequestBackgroundRecording()
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
    }
}