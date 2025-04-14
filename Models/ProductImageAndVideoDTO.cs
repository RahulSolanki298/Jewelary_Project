namespace Models
{
    public class ProductImageAndVideoDTO
    {
        public int Id { get; set; } = 0;

        public string ProductId { get; set; }

        public int? FileId { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public bool? IsDefault { get; set; } = false;

    }
}
