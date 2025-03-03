using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Orders
    {
        [Key]
        public Guid OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string BuyerCode { get; set; }  //   B2B Customer

        public string CustomerCode { get; set; } //  Customer

        public int NoOfQty { get; set; }
        
        public decimal NetAmount { get; set; }

        public string PaymentStatus { get; set; }  //  Payment Pending, Payment 

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
