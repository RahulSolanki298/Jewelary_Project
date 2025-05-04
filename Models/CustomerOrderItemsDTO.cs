using System;

namespace Models
{
    public class CustomerOrderItemsDTO
    {
        public string Id { get; set; }

        public string OrderId { get; set; }

        public string CustomerCode { get; set; }

        public string ProductId { get; set; }

        public int? DiamondId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string CoupanCode { get; set; }

        public int? CoupanId { get; set; }

        public decimal? Dicount { get; set; }

        public decimal? DicountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
