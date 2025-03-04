using System;

namespace DataAccess.Entities
{
    public class CustomerOrderStatus
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string CustomerCode { get; set; }

        public int LastStatusId { get; set; }

        public int CurrentStatusId { get; set; }

        public string Description { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }
    }
}
