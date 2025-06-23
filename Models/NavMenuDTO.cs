using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Models
{
    public class NavMenuDTO
    {
        public IEnumerable<CategoryDTO> CategoryList { get; set; }
        public IEnumerable<ProductStyleDTO> SubcategoryList { get; set; }
        public IEnumerable<ProductCollectionDTO> ProductCollectionList { get; set; }
        public IEnumerable<ProductPropertyDTO> ProductShapeList { get; set; }
        public IEnumerable<DiamondShapeData> DiamondShapeList { get; set; }
    }


}
