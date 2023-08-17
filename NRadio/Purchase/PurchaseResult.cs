using Windows.Services.Store;

namespace NRadio.Purchase
{
    public class PurchaseResult
    {
        public StorePurchaseStatus Status { get; private set; }

        public PurchaseResult(StorePurchaseStatus status)
        {
            Status = status;
        }
    }
}
