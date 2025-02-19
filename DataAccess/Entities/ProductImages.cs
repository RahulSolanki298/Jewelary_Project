namespace DataAccess.Entities
{
    public class ProductImages
    {
        public int Id { get; set; }
        public string ProductId { get; set; }

        public int? ImageLgId { get; set; }

        public int? ImageMdId { get; set; }

        public int? ImageSmId { get; set; }

        public int ImageIndexNumber { get; set; } = 0;

        public int? VideoId { get; set; }

        public bool IsDefault { get; set; }

    }
}
