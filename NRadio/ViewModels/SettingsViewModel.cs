using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Services;
using NRadio.Helpers;
using NRadio.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private ElementTheme elementTheme = ThemeSelectorService.Theme;
        private string versionDescription;
        private UserViewModel user;
        private ICommand switchThemeCommand;
        private ICommand switchLanguageCommand;
        private ICommand logoutCommand;
        private ICommand updateStationsCommand;
        private ICommand buyPremiumCommand;

        public SettingsViewModel() { }

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
        public UserViewModel User
        {
            get => user;
            set => SetProperty(ref user, value);
        }

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
        public ICommand SwitchLanguageCommand
        {
            get
            {
                if (switchLanguageCommand == null)
                {
                    switchLanguageCommand = new RelayCommand<string>(
                        async (param) =>
                        {
                            await LanguageSelectorService.SetLanguageAsync(param);
                        });
                }

                return switchLanguageCommand;
            }
        }
        public ICommand LogoutCommand => logoutCommand ?? (logoutCommand = new RelayCommand(OnLogout));
        public ICommand UpdateStationsCommand => updateStationsCommand ?? (updateStationsCommand = new RelayCommand(UpdateStationsAsync));
        public ICommand BuyPremiumCommand => buyPremiumCommand ?? (buyPremiumCommand = new RelayCommand(async () => await BuyPremium()));

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            IdentityService.LoggedOut += OnLoggedOut;
            UserDataService.UserDataUpdated += OnUserDataUpdated;
            User = await UserDataService.GetUserAsync();
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

        private async Task BuyPremium() { } // TODO: Realize it when i will add purchase
  
        private void OnUserDataUpdated(object sender, UserViewModel userData) => User = userData;
        private async void OnLogout() => await IdentityService.LogoutAsync();
        private void OnLoggedOut(object sender, EventArgs e) => UnregisterEvents();

        private async void UpdateStationsAsync() => await RadioStationsLoader.UpdateRadioStations();
    }
}
