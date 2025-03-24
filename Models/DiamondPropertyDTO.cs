namespace Models
{
    public class DiamondPropertyDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentProperty { get; set; }

        public int? ParentId { get; set; }

        public string IconPath { get; set; }

        public string SymbolName { get; set; }

        public bool IsActivated { get; set; }

    }
}
