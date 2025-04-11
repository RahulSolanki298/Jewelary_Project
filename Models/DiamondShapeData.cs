namespace Models
{
    public class DiamondShapeData
    {
        //Id,Name,IconPath,SymbolName,DispOrder,ParentId 
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public string IconPath { get; set; }

        public int DispOrder { get; set; }

    }
}
