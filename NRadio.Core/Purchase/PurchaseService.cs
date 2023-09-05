namespace NRadio.Core.Purchase
{
    public static class PurchaseService
    {
        // TODO: Change to StoreContextProvider when app will be in Dev Center
        public static readonly IPurchaseProvider PurchaseProvider = new PurchaseSimulatorProvider();
    }
}
