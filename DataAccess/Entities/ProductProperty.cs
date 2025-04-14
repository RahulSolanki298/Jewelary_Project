namespace DataAccess.Entities
{
    public class ProductProperty
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SymbolName { get; set; }

        public string IconPath { get; set; }

        public int? ParentId { get; set; }

        public int DisplayOrder { get; set; }

        public bool? IsActive { get; set; }

    }
}
