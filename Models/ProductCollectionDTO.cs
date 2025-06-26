namespace Models
{
    public class ProductCollectionDTO
    {
        public int Id { get; set; }
        public string CollectionName { get; set; }
        public string Descriptions { get; set; }
        public string CollectionImage { get; set; }
        public bool? IsActivated { get; set; } = false;
        public bool IsCategory { get; set; }
        public string CategoryName { get; set; }
    }
}
