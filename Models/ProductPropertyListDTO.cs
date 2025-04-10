using System.Collections.Generic;

namespace Models
{
    public class ProductPropertyListDTO
    {
        public List<ProductPropertyDTO> Colors { get; set; }

        public List<ProductPropertyDTO> CaratSizes { get; set; }

        public List<ProductCollectionDTO> CollectionList { get; set; }

        public List<ProductStyleDTO> StylesList { get; set; }

    }
}
