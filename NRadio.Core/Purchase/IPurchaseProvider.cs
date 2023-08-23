using System.Threading.Tasks;

namespace NRadio.Purchase
{
    public interface IPurchaseProvider
    {
        Task<PurchaseResult> PurchaseAsync(string productId);
        Task<bool> CheckIfUserHasPremiumAsync();
    }
}