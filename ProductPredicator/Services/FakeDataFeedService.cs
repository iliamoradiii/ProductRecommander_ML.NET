using ProductPredicator.Models;
using ProductPredicator.Services.Interfaces;

namespace ProductPredicator.Services
{
    public class FakeDataFeedService : IFakeDataFeedService
    {
        public IEnumerable<ProductRating> GetTrainingData()
        {
            var rnd = new Random();

            var users = new[] { "U1", "U2", "U3", "U4", "U5", "U6", "U7", "U8", "U9", "U10" };
            var products = new[] { "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10" };

            var data = new List<ProductRating>();

            foreach (var user in users)
            {
                foreach (var product in products)
                {
                    float rating;
                    if (user is "U1" or "U2")
                        rating = product switch
                        {
                            "P1" or "P2" or "P3" => rnd.Next(4, 6),
                            _ => rnd.Next(1, 4)
                        };
                    else if (user is "U3" or "U4")
                        rating = product switch
                        {
                            "P4" or "P5" => rnd.Next(4, 6),
                            _ => rnd.Next(1, 4)
                        };
                    else if (user is "U5" or "U6")
                        rating = product switch
                        {
                            "P6" or "P7" or "P8" => rnd.Next(4, 6),
                            _ => rnd.Next(1, 4)
                        };
                    else
                        rating = rnd.Next(1, 6);

                    data.Add(new ProductRating
                    {
                        UserId = user,
                        ProductId = product,
                        Label = rating
                    });
                }
            }

            return data;
        }
    }
}