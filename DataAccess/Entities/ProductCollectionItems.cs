namespace DataAccess.Entities
{
    public class ProductCollectionItems
    {
        public int Id { get; set; }

        public string ProductId { get; set; }

        public string UserId { get; set; }

        public bool IsActive { get; set; }

        public bool IsHomePage { get; set; }

        public int Index { get; set; }

    }
}
