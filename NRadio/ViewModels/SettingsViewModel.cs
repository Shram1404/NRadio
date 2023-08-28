using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Services;
using NRadio.Helpers;
using NRadio.Services;
using Windows.ApplicationModel;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private ElementTheme elementTheme = ThemeSelectorService.Theme;
        private string versionDescription;
        private bool voiceControlAllowed;
        private UserViewModel user;
        private string currentLanguage;
        private ICommand switchThemeCommand;
        private ICommand switchLanguageCommand;
        private ICommand logoutCommand;
        private ICommand updateStationsCommand;
        private ICommand buyPremiumCommand;
        private ICommand switchVoiceControlCommand;

        public SettingsViewModel()
        {
            InitializeAsync();
        }

        private UserDataService UserDataService => Singleton<UserDataService>.Instance;
        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        public ElementTheme ElementTheme
        {
            get => elementTheme;
            set => SetProperty(ref elementTheme, value);
        }
        public string VersionDescription
        {
            get => versionDescription;
            set => SetProperty(ref versionDescription, value);
        }
        public bool VoiceControlAllowed
        {
            get => voiceControlAllowed;
            set => SetProperty(ref voiceControlAllowed, value);
        }
        public UserViewModel User
        {
            get => user;
            set => SetProperty(ref user, value);
        }
        public string CurrentLanguage
        {
            get
            {
                currentLanguage = LanguageSelectorService.GetCurrentLanguageName();
                return currentLanguage;
            }
            set => SetProperty(ref currentLanguage, value);
        }
        public bool VoiceControlParameter => VoiceControlAllowed;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (switchThemeCommand == null)
                {
                    switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return switchThemeCommand;
            }
        }

        public ICommand SwitchLanguageCommand => switchLanguageCommand ?? (switchLanguageCommand
            = new AsyncRelayCommand<string>(OnLanguageChangeAsync));

        public ICommand LogoutCommand => logoutCommand ?? (logoutCommand
            = new RelayCommand(OnLogout));

        public ICommand UpdateStationsCommand => updateStationsCommand ?? (updateStationsCommand
            = new AsyncRelayCommand(ConfirmUpdateStationsAsync));

        public ICommand BuyPremiumCommand => buyPremiumCommand ?? (buyPremiumCommand = new AsyncRelayCommand(async ()
            => await BuyPremium()));

        public ICommand SwitchVoiceControlCommand => switchVoiceControlCommand ?? (switchVoiceControlCommand
            = new AsyncRelayCommand(OnSwitchVoiceControl));

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            IdentityService.LoggedOut += OnLoggedOut;
            UserDataService.UserDataUpdated += OnUserDataUpdated;
            User = await UserDataService.GetUserAsync();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            VoiceControlAllowed = localSettings.Values.ContainsKey("VoiceControlAllowed") && (bool)localSettings.Values["VoiceControlAllowed"];
        }

        public void UnregisterEvents()
        {
            IdentityService.LoggedOut -= OnLoggedOut;
            UserDataService.UserDataUpdated -= OnUserDataUpdated;
        }

        private string GetVersionDescription()
        {
            string appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async Task BuyPremium()
        {
            var purchaseProvider = ((App)Application.Current).purchaseProvider;
            var result = await purchaseProvider.PurchaseAsync("Premium");

            if (result.Status == StorePurchaseStatus.Succeeded)
            {
                await DialogService.PurchaseCompleteDialogAsync();
            }
            else if (result.Status == StorePurchaseStatus.AlreadyPurchased)
            {
                await DialogService.AlreadyPurchasedDialogAsync();
            }
            else
            {
                await DialogService.PurchaseFailedDialogAsync();
            }
        }

        private async Task OnLanguageChangeAsync(string langCode)
        {
            if (langCode != null)
            {
                await LanguageSelectorService.SetLanguageAsync(langCode);
                CurrentLanguage = LanguageSelectorService.GetCurrentLanguageName();
                //NavigationService.Refresh();
                //NavigationService.RefreshShellPage(); // RefreshShellPage is not working
                await RestartApp();
            }
        }

        private void OnUserDataUpdated(object sender, UserViewModel userData) => User = userData;
        private async void OnLogout() => await IdentityService.LogoutAsync();
        private void OnLoggedOut(object sender, EventArgs e) => UnregisterEvents();

        private async Task ConfirmUpdateStationsAsync()
        {
            if (await DialogService.ConfirmStationsUpdateDialogAsync() == ContentDialogResult.Primary)
            {
                await RadioStationsLoader.UpdateRadioStationsAsync();
            }
        }

        private async Task OnSwitchVoiceControl()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            VoiceControlAllowed = !VoiceControlAllowed;
            localSettings.Values["VoiceControlAllowed"] = VoiceControlAllowed;

            if (VoiceControlAllowed)
            {
                await DialogService.ShowVoiceCommandsListDialogAsync();
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

        private async Task RestartApp() => await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(string.Empty);
    }
}