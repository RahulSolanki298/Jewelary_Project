using System;

namespace DataAccess.Entities
{
    public class ProductMaster
    {
        public int Id { get; set; } //

        public string Title { get; set; }

        public decimal? Price { get; set; }

        public int CategoryId { get; set; }

        public int CenterShapeId { get; set; }

        public int ColorId { get; set; }

        public string ColorName { get; set; }

        public string Sku { get; set; }

        public string GroupId { get; set; }  //  PLDR-381-SE

        public string ProductKey { get; set; } 

        public bool IsActive { get; set; }

        public string ProductStatus { get; set; }  // Hold -- Deactived -- Active

        public bool IsSale { get; set; } = false;

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? FileHistoryId { get; set; }
    }
}
