using Microsoft.ML;
using Microsoft.ML.Trainers;
using ProductPredicator.Models;
using ProductPredicator.Services.Interfaces;

namespace ProductPredicator.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly MLContext _mlContext;
        private ITransformer? _model;
        private PredictionEngine<Purchase, Prediction>? _predictionEngine;

        private readonly List<ProductRating> _trainingData;
        private readonly IFakeDataFeedService _dataFeedService;

        public RecommendationService(IFakeDataFeedService dataFeedService)
        {
            _mlContext = new MLContext(seed: 0);
            _dataFeedService = dataFeedService;
            _trainingData = _dataFeedService.GetTrainingData().ToList();
        }

        public void AddPurchase(Purchase purchase)
        {
            _trainingData.Add(new ProductRating
            {
                UserId = purchase.UserId,
                ProductId = purchase.ProductId,
                Label = purchase.Label
            });
        }

        public IEnumerable<string> GetKnownProducts()
        {
            return _trainingData
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();
        }

        public string Train()
        {
            if (!_trainingData.Any())
                return "No data to train.";

            var dataView = _mlContext.Data.LoadFromEnumerable(_trainingData.Select(x => new Purchase
            {
                UserId = x.UserId,
                ProductId = x.ProductId,
                Label = x.Label
            }));

            var pipeline = _mlContext.Transforms.Conversion
                .MapValueToKey(outputColumnName: "UserIdEncoded", inputColumnName: nameof(Purchase.UserId))
                .Append(_mlContext.Transforms.Conversion
                    .MapValueToKey(outputColumnName: "ProductIdEncoded", inputColumnName: nameof(Purchase.ProductId)))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                    new MatrixFactorizationTrainer.Options
                    {
                        MatrixColumnIndexColumnName = "UserIdEncoded",
                        MatrixRowIndexColumnName = "ProductIdEncoded",
                        LabelColumnName = nameof(Purchase.Label),
                        NumberOfIterations = 50,
                        ApproximationRank = 100
                    }));


            _model = pipeline.Fit(dataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<Purchase, Prediction>(_model);

            return "Model trained successfully.";
        }

        public float Predict(string userId, string productId)
        {
            if (_predictionEngine == null)
                throw new InvalidOperationException("Model not trained. Call Train() first.");

            var prediction = _predictionEngine.Predict(new Purchase
            {
                UserId = userId,
                ProductId = productId,
                Label = 0f
            });

            return float.IsNaN(prediction.Score) || float.IsInfinity(prediction.Score)
                ? 0f
                : prediction.Score;
        }

        public IEnumerable<(string productId, float score)> RecommendTopN(string userId, int topN = 5)
        {
            if (_predictionEngine == null)
                throw new InvalidOperationException("Model not trained. Call Train() first.");

            return GetKnownProducts()
                .Select(p => (productId: p, score: Predict(userId, p)))
                .OrderByDescending(x => x.score)
                .Take(topN)
                .ToList();
        }

        public IReadOnlyList<Purchase> GetPurchases()
        {
            return _trainingData
                .Select(p => new Purchase
                {
                    UserId = p.UserId,
                    ProductId = p.ProductId,
                    Label = p.Label
                })
                .ToList();
        }
    }
}