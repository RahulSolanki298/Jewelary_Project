using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class CoupanMaster
    {
        public int Id { get; set; }

        public string CoupanCode { get; set; }

        public string ProductId { get; set; }

        public decimal Discount { get; set; }
    }
}
