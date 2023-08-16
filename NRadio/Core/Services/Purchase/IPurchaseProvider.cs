using System.Threading.Tasks;

namespace NRadio.Core.Services.Purchase
{
    public interface IPurchaseProvider
    {
        Task<PurchaseResult> PurchaseAsync(string productId);
    }
}
