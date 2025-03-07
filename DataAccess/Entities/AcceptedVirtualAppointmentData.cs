using System;

namespace DataAccess.Entities
{
    public class AcceptedVirtualAppointmentData
    {
        public int Id { get; set; }

        public int VirtualMeetingId { get; set; }

        public string CompanyName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Designation { get; set; }

        public string MeetingWith { get; set; }  // Google Meeting, Team Meeting, Zoom Meeting etc

        public string MeetingUrl { get; set; }

        public DateTime? MeetingDate { get; set; }

        public string MeetingTime { get; set; }

        // Representation Person
        // Employee--> Manager, Technical Person and other
        public string EmployeeId { get; set; }

        public string MeetingDescription { get; set; }

        public int NoOfStar { get; set; } // No Of star out of 5

        public string Comment { get;set; } // Client Reply 

        public bool IsActived { get; set; }

    }
}
