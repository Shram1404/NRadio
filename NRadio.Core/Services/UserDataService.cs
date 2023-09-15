using NRadio.Helpers;
using NRadio.Models;
using NRadio.Models.Enum;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace NRadio.Core.Services
{
    public class UserDataService
    {
        private const string userSettingsKey = "IdentityUser";

        private dynamic user;

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        private MicrosoftGraphService MicrosoftGraphService => Singleton<MicrosoftGraphService>.Instance;

        public event EventHandler<dynamic> UserDataUpdated;

        public UserDataService()
        { }

        public void Initialize()
        {
            IdentityService.LoggedIn += OnLoggedIn;
            IdentityService.LoggedOut += OnLoggedOut;
        }

        public async Task<dynamic> GetUserAsync()
        {
            if (user == null)
            {
                user = await GetUserFromCacheAsync();
                if (user == null)
                {
                    user = GetDefaultUserData();
                }
            }

            return user;
        }

        private async void OnLoggedIn(object sender, EventArgs e)
        {
            user = await GetUserFromGraphApiAsync();
            UserDataUpdated?.Invoke(this, user);
        }

        private async void OnLoggedOut(object sender, EventArgs e)
        {
            user = null;
            await ApplicationData.Current.LocalFolder.SaveAsync<User>(userSettingsKey, null);
        }

        private async Task<dynamic> GetUserFromCacheAsync()
        {
            var cacheData = await ApplicationData.Current.LocalFolder.ReadAsync<User>(userSettingsKey);
            return await GetUserViewModelFromData(cacheData);
        }

        private async Task<dynamic> GetUserFromGraphApiAsync()
        {
            var accessToken = await IdentityService.GetAccessTokenForGraphAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return null;
            }

            var userData = await MicrosoftGraphService.GetUserInfoAsync(accessToken);
            if (userData != null)
            {
                userData.Photo = await MicrosoftGraphService.GetUserPhoto(accessToken);
                await ApplicationData.Current.LocalFolder.SaveAsync(userSettingsKey, userData);
            }

            return await GetUserViewModelFromData(userData);
        }

        private async Task<dynamic> GetUserViewModelFromData(User userData)
        {
            if (userData == null)
            {
                return null;
            }

            var userPhoto = string.IsNullOrEmpty(userData.Photo)
                ? ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
                : await ImageHelper.ImageFromStringAsync(userData.Photo);

            var viewModelType = ViewModelLocatorHelper.GetViewModelType(VMLocator.UserVM);
            dynamic userViewModel = Activator.CreateInstance(viewModelType);

            userViewModel.Name = userData.DisplayName;
            userViewModel.UserPrincipalName = userData.UserPrincipalName;
            userViewModel.Photo = userPhoto;

            return userViewModel;
        }

        private dynamic GetDefaultUserData()
        {
            var viewModelType = ViewModelLocatorHelper.GetViewModelType(VMLocator.UserVM);
            dynamic userViewModel = Activator.CreateInstance(viewModelType);


            userViewModel.Name = IdentityService.GetAccountUserName();
            userViewModel.Photo = ImageHelper.ImageFromAssetsFile("DefaultIcon.png");
            return userViewModel;

        }
    }
}
