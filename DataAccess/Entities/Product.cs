using System;

namespace DataAccess.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string VenderId { get; set; }
        public string Sku { get; set; }  // OTN23U01-D300R0B 
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
        public string ProductType { get; set; } // Diamond, Gold

        // If Gold
        public int? GoldPurityId { get; set; } // 18 k
        public int? GoldWeightId { get; set; } // 3.00 GMS
        public int? CTW { get; set; }

        //
        public int? CenterShapeId { get; set; }

        public int? CenterCaratId { get; set; }

        //  If Diamond
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? CaratId { get; set; }
        public int? CaratSizeId { get; set; }
        public int? ClarityId { get; set; }
        public int? ColorId { get; set; }
        public int? ShapeId { get; set; }
        public int? StyleId { get; set; }       // Style is brands
        public int? CollectionsId { get; set; } // Group of products

        public int NoOfStones { get; set; }
        public bool IsActivated { get; set; } = false;
    }
}
