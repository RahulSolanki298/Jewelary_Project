using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ProductFullDetails
    {

        public ProductDTO Product { get; set; }
        public List<ProductImageAndVideoDTO> Imgs { get; set; }
        public List<ProductImageAndVideoDTO> videos { get; set; }
    }
}
