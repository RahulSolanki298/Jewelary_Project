using System;

namespace Models
{
    public class ProductDTO
    {
        public Guid Id { get; set; }

        public string Sku { get; set; }  // OTN23U01-D300R0B 

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; } = 0;

        public string ProductType { get; set; } // Diamond, Gold

        // If Gold
        public string GoldPurity { get; set; } // 18 k

        public string GoldWeight { get; set; } // 3.00 GMS

        //  If Diamond
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int? SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public int? CaratId { get; set; }

        public string CaratName { get; set; }

        public int? CaratSizeId { get; set; }

        public string CaratSizeName { get; set; }

        public int? ClarityId { get; set; }

        public string ClarityName { get; set; }

        public int? ColorId { get; set; }

        public string ColorName { get; set; }

        public int? ShapeId { get; set; }

        public string ShapeName { get; set; }

        public int? StyleId { get; set; }       // Style is brands

        public string StyleName { get; set; }

        public int? CollectionsId { get; set; } // Group of products

        public string CollectionName { get; set; }

        public bool IsActivated { get; set; } = false;
    }
}
