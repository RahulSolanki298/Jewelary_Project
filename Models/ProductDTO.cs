using System;
using System.Collections.Generic;

namespace Models
{
    public class ProductDTO
    {
        public Guid Id { get; set; }

        public DateTime ProductDate { get; set; }
        
        public string VenderId { get; set; }

        public string VenderName { get; set; }
        
        public string DesignNo { get; set; }
        
        public string ParentDesignNo { get; set; }
        public string ParentDesign { get; set; }

        public string ProductSize { get; set; }
        
        public string Gender { get; set; }

        public string Sku { get; set; }  // OTN23U01-D300R0B 

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; } = 0;

        public string ProductType { get; set; } // Diamond, Gold

        public string BandWidth { get; set; }

        public string Karat { get; set; }
        public int? KaratId { get; set; }
        // If Gold
        public string GoldPurity { get; set; } // 18 k

        public string GoldWeight { get; set; } // 3.00 GMS

        public string CTW { get; set; }

        //  If Diamond
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int? SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public string Carat { get; set; }

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

        public string Length { get; set; }

        public int? StyleId { get; set; }       // Style is brands

        public string StyleName { get; set; }

        public int? CollectionsId { get; set; } // Group of products

        public string CollectionName { get; set; }

        public int? CenterShapeId { get; set; }

        public string CenterShapeName { get; set; }

        public int? CenterCaratId { get; set; }

        public string CenterCaratName { get; set; }

        public string DefaultImage { get; set; }
        
        public int NoOfStones { get; set; }
        
        public decimal? DiaWT { get; set; }
        
        public string Grades { get; set; }

        public string WebsiteImagesLink { get; set; }

        public bool IsActivated { get; set; } = true;

        public string Occasion { get; set; }
        
        public string Remarks { get; set; }
        
        public string Package { get; set; }
        
        public string MfgDesign { get; set; }

        public string Designer { get; set; }

        public string CadDesigner { get; set; }

        public string Setting { get; set; }

        public List<ProductImageAndVideoDTO> ProductImageVideos { get; set; }
    }
}
