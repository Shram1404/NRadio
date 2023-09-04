using System;
using System.Threading.Tasks;
using NRadio.Helpers;
using NRadio.Models;
using NRadio.Core.Services;
using NRadio.ViewModels;
using Windows.Storage;

namespace NRadio.Core.Services
{
    public class UserDataService
    {
        private const string userSettingsKey = "IdentityUser";

        private object user;

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        private MicrosoftGraphService MicrosoftGraphService => Singleton<MicrosoftGraphService>.Instance;

        public event EventHandler<object> UserDataUpdated;

        public UserDataService()
        {
        }

        public void Initialize()
        {
            IdentityService.LoggedIn += OnLoggedIn;
            IdentityService.LoggedOut += OnLoggedOut;
        }

        public async Task<object> GetUserAsync()
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

        private async Task<object> GetUserFromCacheAsync()
        {
            var cacheData = await ApplicationData.Current.LocalFolder.ReadAsync<User>(userSettingsKey);
            return await GetUserViewModelFromData(cacheData);
        }

        private async Task<object> GetUserFromGraphApiAsync()
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

        private async Task<object> GetUserViewModelFromData(User userData)
        {
            if (userData == null)
            {
                return null;
            }

            var userPhoto = string.IsNullOrEmpty(userData.Photo)
                ? ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
                : await ImageHelper.ImageFromStringAsync(userData.Photo);

            return new UserViewModel()
            {
                Name = userData.DisplayName,
                UserPrincipalName = userData.UserPrincipalName,
                Photo = userPhoto
            };
        }

        private UserViewModel GetDefaultUserData()
        {
            return new UserViewModel()
            {
                Name = IdentityService.GetAccountUserName(),
                Photo = ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
            };
        }
    }
}
