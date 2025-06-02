namespace DataAccess.Entities
{
    public class ProductStyleItems
    {
        public int Id { get; set; }

        public int? StyleId { get; set; }

        public string ProductId { get; set; }

        public string UserId { get; set; }

        public bool IsActive { get; set; }

        public bool IsHomePage { get; set; }

        public int Index { get; set; }

    }
}
