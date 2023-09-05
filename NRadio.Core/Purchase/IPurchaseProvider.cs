using System.Threading.Tasks;

namespace NRadio.Core.Purchase
{
    public interface IPurchaseProvider
    {
        Task<PurchaseResult> PurchaseAsync(string productId);
        Task<bool> CheckIfUserHasPremiumAsync();
    }
}