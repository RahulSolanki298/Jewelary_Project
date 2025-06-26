using System.Collections.Generic;

namespace Models
{
    public class ProductStyleCollectionDTO
    {
        public bool IsJewellery { get; set; }

        public List<ProductDTO> Product { get; set; }
        
        public bool IsStyle { get; set; }

        public List<ProductStyleDTO> StyleList { get; set; }

        public bool IsCollection { get; set; }

        public List<ProductCollectionDTO> CollectionList { get; set; }

        public bool IsCategory { get; set; }

        public List<CategoryDTO> CategoryList { get; set; }

    }
}
