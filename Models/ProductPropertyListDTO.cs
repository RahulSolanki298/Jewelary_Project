using System.Collections.Generic;

namespace Models
{
    public class ProductPropertyListDTO
    {
        public List<ProductPropertyDTO> Colors { get; set; }

        public List<ProductPropertyDTO> CaratSizes { get; set; }

        public List<SubCategoryDTO> CollectionList { get; set; }

        public List<CategoryDTO> StylesList { get; set; }


    }
}
