﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Orders
    {
        [Key]
        public Guid OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string  OrderCode { get; set; }

        public string BusinessCode { get; set; }  //   B2B Customer

        public int NoOfQty { get; set; }
        
        public decimal NetAmount { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string PaymentStatus { get; set; }  //  Payment Pending, Payment 
    }
}
