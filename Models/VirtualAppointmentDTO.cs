using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VirtualAppointmentDTO
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        // Jewelry like [Ring, Earrings]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public DateTime? RegisterDate { get; set; }

        public string RegisterTime { get; set; } //  hh:mm

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailId { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
    }
}
