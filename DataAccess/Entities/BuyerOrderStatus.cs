using System;

namespace DataAccess.Entities
{
    public class BuyerOrderStatus
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int BuyerId { get; set; }

        public int LastStatusId { get; set; }

        public int CurrentStatusId { get; set; }

        public string Description { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }
    }
}
