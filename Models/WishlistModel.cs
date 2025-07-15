using System.Collections.Generic;

namespace Models
{
    public class WishlistModel
    {
        public IEnumerable<DiamondData> Diamonds { get; set; }
        public IEnumerable<ProductMasterDTO> Jewelleries { get; set; }
    }
}
