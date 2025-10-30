using ProductPredicator.Models;

namespace ProductPredicator.Services.Interfaces
{
    public interface IRecommendationService
    {
        void AddPurchase(Purchase purchase);
        string Train();
        float Predict(string userId, string productId);
        IEnumerable<(string productId, float score)> RecommendTopN(string userId, int topN = 5);
        IReadOnlyList<Purchase> GetPurchases();
        IEnumerable<string> GetKnownProducts();
    }
}
