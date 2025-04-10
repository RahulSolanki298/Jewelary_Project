using System.Collections.Generic;

namespace Models
{
    public class ProductWithImgDT_DTO
    {
        public ProductDTO Product { get; set; }

        public List<ProductImageAndVideoDTO> ProductProperty { get; set; }
    }
}
