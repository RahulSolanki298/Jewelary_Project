namespace Models
{
    public class ProductPriceDTO
    {
        public int Id { get; set; }

        public string ProductId { get; set; }

        public int? KaratId { get; set; }

        public string KaratName { get; set; }

        public decimal? ProductPrice { get; set; } = 0;
    }
}
