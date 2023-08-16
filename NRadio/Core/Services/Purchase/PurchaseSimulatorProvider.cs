using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NRadio.Services;
using Windows.ApplicationModel.Resources;
using Windows.Services.Store;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace NRadio.Core.Services.Purchase
{
    public class PurchaseSimulatorProvider : IPurchaseProvider
    {
        private const int SubscribePeriodInDays = 30;

        public PurchaseSimulatorProvider() { }

        public async Task<PurchaseResult> PurchaseAsync(string productId)
        {
            var result = await DialogService.ConfirmPurchaseDialogAsync(productId, SubscribePeriodInDays);
            if (result == ContentDialogResult.Primary)
            {
                if (await CheckIfUserHasPremiumAsync())
                {
                    return new PurchaseResult(StorePurchaseStatus.AlreadyPurchased);
                }

                try
                {
                    return await StartPurchaseAsync(productId);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to purchase: {ex}");
                    return new PurchaseResult(StorePurchaseStatus.NotPurchased);
                }
            }
            else
            {
                return new PurchaseResult(StorePurchaseStatus.NotPurchased);
            }
        }

        public async Task<bool> CheckIfUserHasPremiumAsync() // Async only for consistency with StoreContextProvider
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Premium")
                && (string)ApplicationData.Current.LocalSettings.Values["Email"] == await GetUserEmailAsync())
            {
                object expirationDateObject = ApplicationData.Current.LocalSettings.Values["Premium_ExpirationDate"];
                if (expirationDateObject != null)
                {
                    long expirationDateTicks = (long)expirationDateObject;
                    var expirationDate = new DateTime(expirationDateTicks);
                    if (DateTime.Now < expirationDate)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task<PurchaseResult> StartPurchaseAsync(string productId)
        {
            string email = await GetUserEmailAsync();
            var expirationDate = DateTime.Now.AddDays(SubscribePeriodInDays);
            long expirationDateTicks = expirationDate.Ticks;

            ApplicationData.Current.LocalSettings.Values[productId] = true;
            ApplicationData.Current.LocalSettings.Values["Email"] = email;
            ApplicationData.Current.LocalSettings.Values[$"{productId}_ExpirationDate"] = expirationDateTicks;

            return new PurchaseResult(StorePurchaseStatus.Succeeded);
        }

        private async Task<string> GetUserEmailAsync()
        {
            var userData = new UserDataService();
            var user = await userData.GetUserAsync();
            return user.UserPrincipalName;
        }
    }
}