namespace DataAccess.Entities
{
    public class ProductPrices
    {
        public int Id { get; set; }

        public string ProductId { get; set; }

        public int? KaratId { get; set; }

        public decimal? ProductPrice { get; set; } = 0;

    }
}
