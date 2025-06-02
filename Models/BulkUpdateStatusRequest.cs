using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BulkUpdateStatusRequest
    {
        public List<string> Ids { get; set; }
        public int styleId { get; set; }
    }

}
