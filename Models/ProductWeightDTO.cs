namespace Models
{
    public class ProductWeightDTO
    {
        public int Id { get; set; }

        public string ProductId { get; set; }

        public int KaratId { get; set; }

        public string KaratName { get; set; }

        public decimal? Weight { get; set; } = 0;
    }
}
