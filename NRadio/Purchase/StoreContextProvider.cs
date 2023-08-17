using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.UI.Xaml.Controls;
using NRadio.Core.Services;

namespace NRadio.Purchase
{
    public class StoreContextProvider : IPurchaseProvider
    {
        private readonly int subscribePeriodInDays = 30;
        private readonly StoreContext storeContext;

        public StoreContextProvider()
        {
            storeContext = StoreContext.GetDefault();
        }

        public async Task<PurchaseResult> PurchaseAsync(string productId)
        {
            var storeConnectionResult = await storeContext.GetStoreProductForCurrentAppAsync();
            if (storeConnectionResult.ExtendedError != null)
            {
                Debug.WriteLine($"Failed to connect to the Store: {storeConnectionResult.ExtendedError.Message}");
            }

            var result = await DialogService.ConfirmPurchaseDialogAsync(productId, subscribePeriodInDays);
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

        public async Task<bool> CheckIfUserHasPremiumAsync()
        {
            var context = StoreContext.GetDefault();
            var appLicense = await context.GetAppLicenseAsync();

            var license = appLicense.AddOnLicenses.FirstOrDefault(l => l.Value.SkuStoreId.StartsWith("Premium") && l.Value.IsActive);
            return license.Value != null;
        }

        private async Task<PurchaseResult> StartPurchaseAsync(string productId)
        {
            var result = await storeContext.RequestPurchaseAsync(productId);
            return new PurchaseResult(result.Status);
        }
    }
}