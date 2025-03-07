using System;

namespace DataAccess.Entities
{
    public class VirtualAppointment
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public int CategoryId { get; set; }

        public DateTime RegisterDate { get; set; }

        public string RegisterTime { get; set; } //  hh:mm

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Designation { get; set; }

        public string EmailId { get; set; }

        public string MobileNumber { get; set; }

        public string Message { get; set; }  // Message for Accept and Reject status 

        public string Status { get; set; }

    }
}
