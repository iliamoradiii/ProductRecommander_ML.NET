namespace ProductPredicator.Models
{
    public class Purchase
    {
        public string UserId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public float Label { get; set; } // 1 = bought, 0 = not bought
    }
}
