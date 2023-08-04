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
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.ViewModels
{
    // TODO: Add other settings as necessary. For help see https://github.com/microsoft/TemplateStudio/blob/main/docs/UWP/pages/settings.md
    public class SettingsViewModel : ObservableObject
    {
        private UserDataService UserDataService => Singleton<UserDataService>.Instance;

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        private UserViewModel _user;

        private ICommand _logoutCommand;

        public ICommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new RelayCommand(OnLogout));

        public UserViewModel User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private ICommand _updateStationsCommand;
        public ICommand UpdateStationsCommand => _updateStationsCommand ?? (_updateStationsCommand = new RelayCommand(UpdateStationsAsync));

        private ICommand _buyPremiumCommand;
        public ICommand BuyPremiumCommand => _buyPremiumCommand ?? (_buyPremiumCommand = new RelayCommand(async () => await BuyPremium()));


        public SettingsViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            IdentityService.LoggedOut += OnLoggedOut;
            UserDataService.UserDataUpdated += OnUserDataUpdated;
            User = await UserDataService.GetUserAsync();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async Task BuyPremium()
        {
            if (!((App)Application.Current).licenseInformation.ProductLicenses["Premium"].IsActive)
            {
                try
                {
                    await CurrentAppSimulator.RequestProductPurchaseAsync("Premium");
                    if (((App)Application.Current).licenseInformation.ProductLicenses["Premium"].IsActive)
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Преміум-ліцензія активована",
                            Content = "Ви успішно активували преміум-ліцензію. Ви можете використовувати всі функції додатку.",
                            CloseButtonText = "ОК"
                        };
                        await dialog.ShowAsync();
                    }
                    if (!((App)Application.Current).licenseInformation.ProductLicenses["Premium"].IsActive)
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Преміум-ліцензія не активована",
                            Content = "Преміум-ліцензія не була активована. Спробуйте пізніше.",
                            CloseButtonText = "ОК"
                        };
                        await dialog.ShowAsync();
                    }
                }
                catch (Exception)
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Помилка під час активації",
                        Content = "Відбулась помилки при спробі активації преміум-ліцензії. Спробуйте пізніше.",
                        CloseButtonText = "ОК"
                    };
                    await dialog.ShowAsync();
                }
            }
            else
            {

            }
        }

        public void UnregisterEvents()
        {
            IdentityService.LoggedOut -= OnLoggedOut;
            UserDataService.UserDataUpdated -= OnUserDataUpdated;
        }

        private void OnUserDataUpdated(object sender, UserViewModel userData) => User = userData;
        private async void OnLogout() => await IdentityService.LogoutAsync();
        private void OnLoggedOut(object sender, EventArgs e) => UnregisterEvents();

        private async void UpdateStationsAsync() => await RadioStationsLoader.UpdateRadioStations();
    }
}
