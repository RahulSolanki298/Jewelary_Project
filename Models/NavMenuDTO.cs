using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Models
{
    public class NavMenuDTO
    {
        public List<CategoryDTO> CategoryList { get; set; }
        public List<ProductStyleDTO> SubcategoryList { get; set; }
        public List<ProductCollectionDTO> ProductCollectionList { get; set; }
        public List<ProductPropertyDTO> ProductShapeList { get; set; }
        public List<DiamondPropertyDTO> DiamondShapeList { get; set; }
    }

}
