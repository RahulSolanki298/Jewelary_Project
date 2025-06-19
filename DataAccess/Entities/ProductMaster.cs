namespace DataAccess.Entities
{
    public class ProductMaster
    {
        public int Id { get; set; } //

        public string GroupId { get; set; }  //  PLDR-381-SE

        public string ProductKey { get; set; } //  PLDR-381-SE-Y

        public bool IsActive { get; set; }

        public string ProductStatus { get; set; }  // Hold -- Deactived -- Active

        public bool IsSale { get; set; } = false;


    }
}
