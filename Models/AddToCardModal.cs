using System.Collections.Generic;

namespace Models
{
    public class AddToCardModal
    {
        public IEnumerable<DiamondData> Diamonds { get; set; }
        public IEnumerable<ProductDTO> Jewelleries { get; set; }

    }
}
