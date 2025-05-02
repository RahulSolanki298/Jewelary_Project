using System;

namespace Models
{
    public class CompanyDataDTO
    {
        public int Id { get; set; }

        public string VendarId { get; set; }

        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string CompanyLogo { get; set; }

        public string ZipCode { get; set; }

        public string Registration_Number { get; set; }

        public DateTime? Founded_Date { get; set; }

        public string Website { get; set; }

        public string EmailId { get; set; }

        public string PhoneNo1 { get; set; }

        public string PhoneNo2 { get; set; }

        public string CityName { get; set; }

        public string StateName { get; set; }

        public string CountryName { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

    }
}
