using Microsoft.Extensions.DependencyInjection;
using NRadio.Core.Helpers;
using NRadio.Core.Services;
using NRadio.Services;
using NRadio.ViewModels;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;

namespace NRadio
{
    public sealed partial class App : Application
    {
        internal LicenseInformation licenseInformation;

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        private ServiceProvider _serviceProvider;
        public ViewModelLocator ViewModelLocator => _serviceProvider.GetService<ViewModelLocator>();

        public App()
        {
            InitializeComponent();

            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;
            UnhandledException += OnAppUnhandledException;

            _activationService = new Lazy<ActivationService>(CreateActivationService);
            IdentityService.LoggedOut += OnLoggedOut;

            RegisterServices();

            licenseInformation = CurrentAppSimulator.LicenseInformation;
        }


        private void RegisterServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ViewModelLocator>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<ShellViewModel>();
            services.AddSingleton<PlayerViewModel>();
            services.AddSingleton<StationDetailViewModel>();
            services.AddSingleton<StationsListViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            await RadioStationsLoader.Initialize();

            var backgroundTaskService = new BackgroundTaskService();
            await backgroundTaskService.RegisterBackgroundTasksAsync();
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Unhandled exception: " + e.Message);
            System.Diagnostics.Debug.WriteLine(e.Exception.StackTrace);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

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

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private async void OnLoggedOut(object sender, EventArgs e)
        {
            ActivationService.SetShell(new Lazy<UIElement>(CreateShell));
            await ActivationService.RedirectLoginPageAsync();
        }
    }
}
