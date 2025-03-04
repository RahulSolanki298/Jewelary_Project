using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        public string EmailId { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

    }
}
