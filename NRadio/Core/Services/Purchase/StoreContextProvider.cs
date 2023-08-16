using System;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace NRadio.Core.Services.Purchase
{
    public class StoreContextProvider : IPurchaseProvider
    {
        private readonly StoreContext storeContext;

        public StoreContextProvider()
        {
            storeContext = StoreContext.GetDefault();
        }

        public async Task<PurchaseResult> PurchaseAsync(string productId)
        {
            var result = await storeContext.RequestPurchaseAsync(productId);
            return new PurchaseResult(result.Status);
        }
        public static async Task<bool> CheckIfUserHasPremiumAsync()
        {
            var context = StoreContext.GetDefault();
            var appLicense = await context.GetAppLicenseAsync();

            foreach (var addOnLicense in appLicense.AddOnLicenses)
            {
                var license = addOnLicense.Value;
                if (license.SkuStoreId.StartsWith("Premium") && license.IsActive)
                {
                    return true;
                }
            }

            return false;
        }
    }
}