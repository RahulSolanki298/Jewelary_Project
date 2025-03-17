namespace DataAccess.Entities
{
    public class ProductProperty
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SymbolName { get; set; }

        public string IconPath { get; set; }

        public int? ParentId { get; set; }

    }
}
