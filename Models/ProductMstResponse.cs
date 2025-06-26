using System.Collections.Generic;

namespace Models
{
    public class ProductMstResponse
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public List<ProductDTO> tempProductList { get; set; }
    }
}
