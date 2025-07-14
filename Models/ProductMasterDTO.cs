using System.Collections.Generic;

namespace Models
{
    public class ProductMasterDTO
    {
        public int Id { get; set; }

        public int? ColorId { get; set; }
        public string ColorName { get; set; }

        public string GroupId { get; set; }  //  PLDR-381-SE

        public string ProductKey { get; set; } //  PLDR-381-SE-Y

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int ShapeId { get; set; }

        public string ShapeName { get; set; }

        public string Title { get; set; }

        public decimal? Price { get; set; }

        public bool IsActive { get; set; }

        public string ProductStatus { get; set; }  // Hold -- Deactived -- Active

        public bool IsSale { get; set; } = false;

        public string Sku { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public List<ProductDTO> ProductItems { get; set; }  // detail
        public List<ProductImageAndVideoDTO> ProductImageVideos { get; set; }
        public List<ProductPropertyDTO> Metals { get; set; }
        public List<ProductPropertyDTO> CaratSizes { get; set; }
        public List<ProductPropertyDTO> Shapes { get; set; }
    }
}
