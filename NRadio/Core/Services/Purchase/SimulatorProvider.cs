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
    public class SimulatorProvider : IPurchaseProvider
    {
        private const int SubscribePeriodInDays = 30;

        public SimulatorProvider() { }

        public async Task<PurchaseResult> PurchaseAsync(string productId)
        {
            var result = await ConfirmDialog(productId);
            if (result == ContentDialogResult.Primary)
            {
                if (await CheckIfUserHasPremiumAsync())
                {
                    return new PurchaseResult(StorePurchaseStatus.AlreadyPurchased);
                }

                try
                {
                    string email = await GetUserEmail();
                    var expirationDate = DateTime.Now.AddDays(SubscribePeriodInDays);
                    long expirationDateTicks = expirationDate.Ticks;

                    ApplicationData.Current.LocalSettings.Values[productId] = true;
                    ApplicationData.Current.LocalSettings.Values["Email"] = email;
                    ApplicationData.Current.LocalSettings.Values[$"{productId}_ExpirationDate"] = expirationDateTicks;

                    return new PurchaseResult(StorePurchaseStatus.Succeeded);
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
                && (string)ApplicationData.Current.LocalSettings.Values["Email"] == await GetUserEmail())
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

        private async Task<ContentDialogResult> ConfirmDialog(string productId)
        {
            var loader = new ResourceLoader();
            string titleRes = loader.GetString($"Premium_SimulatorBuyConfirm/Title");
            string title = string.Format(titleRes, productId);
            string content = loader.GetString("Premium_SimulatorBuyConfirm/Content");
            string yesButtonText = loader.GetString("Premium_SimulatorBuyConfirm/Ok");
            string noButtonText = loader.GetString("Premium_SimulatorBuyConfirm/No");

            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = yesButtonText,
                CloseButtonText = noButtonText
            };

            return await dialog.ShowAsync();
        }

        private async Task<string> GetUserEmail()
        {
            var userData = new UserDataService();
            var user = await userData.GetUserAsync();
            return user.UserPrincipalName;
        }
    }
}