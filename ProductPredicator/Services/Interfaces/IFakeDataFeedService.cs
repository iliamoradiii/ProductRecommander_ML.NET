using ProductPredicator.Models;

namespace ProductPredicator.Services.Interfaces
{
    public interface IFakeDataFeedService
    {
        IEnumerable<ProductRating> GetTrainingData();
    }
}
