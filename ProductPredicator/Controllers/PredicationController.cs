using Microsoft.AspNetCore.Mvc;
using ProductPredicator.Models;
using ProductPredicator.Services.Interfaces;

namespace ProductPredicator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredicationController: ControllerBase
    {
        private readonly IRecommendationService _recommandationService;
        public PredicationController(IRecommendationService svc) => _recommandationService = svc;

        // POST api/purchases/add
        [HttpPost("add")]
        public IActionResult Add([FromBody] PurchaseDto dto)
        {
            _recommandationService.AddPurchase(new Purchase { UserId = dto.UserId, ProductId = dto.ProductId, Label = 1f });
            return Ok(new { message = "Added" });
        }

        // POST api/purchases/train
        [HttpPost("train")]
        public IActionResult Train()
        {
            var res = _recommandationService.Train();
            return Ok(new { message = res });
        }

        // GET api/purchases/predict?userId=u1&productId=p1
        [HttpGet("predict")]
        public IActionResult Predict([FromQuery] string userId, [FromQuery] string productId)
        {
            try
            {
                var score = _recommandationService.Predict(userId, productId);
                return Ok(new { userId, productId, score });
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        // GET api/purchases/recommend/{userId}?topN=5
        [HttpGet("recommend/{userId}")]
        public IActionResult Recommend(string userId, [FromQuery] int topN = 5)
        {
            try
            {
                var list = _recommandationService.RecommendTopN(userId, topN)
                    .Select(x => new { ProductId = x.productId, Score = x.score });
                return Ok(list);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        // GET api/purchases/all
        [HttpGet("all")]
        public IActionResult All() => Ok(_recommandationService.GetPurchases());

    }
}
