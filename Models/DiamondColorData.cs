namespace Models
{
    public class DiamondColorData
    {
        // Id,Name,IconPath,SymbolName,DispOrder,ParentId
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public string IconPath { get; set; }

        public string SymbolName { get; set; }

        public int DispOrder { get; set; }
    }
}
