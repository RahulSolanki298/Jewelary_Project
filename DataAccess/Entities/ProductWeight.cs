namespace DataAccess.Entities
{
    public class ProductWeight
    {
        public int Id { get; set; }

        public string ProductId { get; set; }

        public int KaratId { get; set; }

        public decimal? Weight { get; set; } = 0;

    }
}
