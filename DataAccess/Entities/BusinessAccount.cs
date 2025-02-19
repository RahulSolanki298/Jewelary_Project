using System;

namespace DataAccess.Entities
{
    public class BusinessAccount 
    {
        public int Id { get; set; }

        public string BusinessCode { get; set; }

        public string CompanyName { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public int CompanyLogoId { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string ContactNumber { get; set; }

        public string BusinessEmailId { get; set; }

        public string WhatsAppNumber { get; set; }

        public string OfficialWebsite { get; set; }

        public DateTime RegisterDate { get; set; }

        public string BusinessCertificate { get; set; }
        
        public string BusinessPanCardNo { get; set; }
        
        public string BusinessAadharCardNo { get; set; }

        public string BusinessAccountType { get; set; }  //  Silver, Gold, premium account

        public int OwnerProfileImageId { get; set; }

        public bool IsActivated { get; set; }

    }
}
