using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NRadio.Services;
using NRadio.Core.Helpers;
using NRadio.Core.Services;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Activation
{
    internal class ActivationService
    {
        private readonly App app;
        private readonly Type defaultNavItem;
        private Lazy<UIElement> shell;
        private object lastActivationArgs;

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        {
            this.app = app;
            this.shell = shell;
            this.defaultNavItem = defaultNavItem;
            IdentityService.LoggedIn += OnLoggedIn;
        }

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;
        private UserDataService UserDataService => Singleton<UserDataService>.Instance;

        public async Task RedirectLoginPageAsync()
        {
            var frame = new Frame();
            NavigationService.Frame = frame;
            Window.Current.Content = frame;
            await LanguageSelectorService.SetRequestedLanguageAsync();
            await ThemeSelectorService.SetRequestedThemeAsync();
            NavigationService.Navigate<Views.LogInPage>();
        }

        public void SetShell(Lazy<UIElement> shell)
        {
            this.shell = shell;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                await InitializeAsync();
                UserDataService.Initialize();
                IdentityService.InitializeWithAadAndPersonalMsAccounts();
                var silentLoginSuccess = await IdentityService.AcquireTokenSilentAsync();
                if (!silentLoginSuccess || !IdentityService.IsAuthorized())
                {
                    await RedirectLoginPageAsync();
                }

                if (Window.Current.Content == null)
                {
                    Window.Current.Content = shell?.Value ?? new Frame();
                }
            }

            if (IdentityService.IsLoggedIn())
            {
                await HandleActivationAsync(activationArgs);
            }

            lastActivationArgs = activationArgs;

            if (IsInteractive(activationArgs))
            {
                var activation = activationArgs as IActivatedEventArgs;
                if (activation.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    await Singleton<SuspendAndResumeService>.Instance.RestoreSuspendAndResumeData();
                }

                Window.Current.Activate();
                await StartupAsync();
            }
        }

        private async void OnLoggedIn(object sender, EventArgs e)
        {
            if (shell?.Value != null)
            {
                Window.Current.Content = shell.Value;
            }
            else
            {
                var frame = new Frame();
                Window.Current.Content = frame;
                NavigationService.Frame = frame;
            }

            await LanguageSelectorService.SetRequestedLanguageAsync();
            await ThemeSelectorService.SetRequestedThemeAsync();
            await HandleActivationAsync(lastActivationArgs);
        }

        private async Task InitializeAsync()
        {
            await Singleton<BackgroundTaskService>.Instance.RegisterBackgroundTasksAsync().ConfigureAwait(false);
            await LanguageSelectorService.InitializeAsync().ConfigureAwait(false);
            await ThemeSelectorService.InitializeAsync().ConfigureAwait(false);
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                var defaultHandler = new DefaultActivationHandler(defaultNavItem);
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs);
                }
            }
        }

        private async Task StartupAsync()
        {
            await LanguageSelectorService.SetRequestedLanguageAsync();
            await ThemeSelectorService.SetRequestedThemeAsync();
            await FirstRunDisplayService.ShowIfAppropriateAsync();
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<BackgroundTaskService>.Instance;
            yield return Singleton<SuspendAndResumeService>.Instance;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }
    }
}
