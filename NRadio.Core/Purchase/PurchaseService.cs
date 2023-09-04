using NRadio.Purchase;

namespace NRadio.Core.Purchase
{
    public static class PurchaseService
    {
        // TODO: Change to StoreContextProvider when app wil be in Dev Center
        public static readonly IPurchaseProvider PurchaseProvider = new PurchaseSimulatorProvider();
    }
}
