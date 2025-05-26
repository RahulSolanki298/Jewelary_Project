using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class CustomerOrders
    {
        [Key]
        public Guid OrderId { get; set; }

        public DateTime? OrderDate { get; set; }

        public string  OrderCode { get; set; }

        public string CustomerCode { get; set; } //  Customer

        public string CoupanCode { get; set; }

        public int? CoupanId { get; set; }

        public decimal? Dicount { get; set; }

        public decimal? DicountAmount { get; set; }

        public int NoOfQty { get; set; }
        
        public decimal NetAmount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string PaymentStatus { get; set; }  //  Payment Pending, Payment 
    }
}
