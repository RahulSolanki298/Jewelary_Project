using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class BusinessAccountRegisterDTO
    {
        public CustomerRegisterDTO Customer { get; set; }
        public int Id { get; set; }

        public string BusinessCode { get; set; }

        [Required(ErrorMessage ="Please enter company name.")]
        public string CompanyName { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        [Required(ErrorMessage = "Please enter company logo.")]
        public int CompanyLogoId { get; set; }

        [Required(ErrorMessage = "Please enter address line 1.")]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage = "Please enter address line 2.")]
        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Please enter contact number.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Please enter business email id.")]
        public string BusinessEmailId { get; set; }

        [Required(ErrorMessage = "Please enter whatsapp number.")]
        public string WhatsAppNumber { get; set; }
 
        public string OfficialWebsite { get; set; }

        [Required(ErrorMessage = "Please enter register date.")]
        public DateTime RegisterDate { get; set; }

        [Required(ErrorMessage = "Please enter business certificate.")]
        public string BusinessCertificate { get; set; }

        [Required(ErrorMessage = "Please enter business pancard no.")]
        public string BusinessPanCardNo { get; set; }

        [Required(ErrorMessage = "Please enter business aadharcard no.")]
        public string BusinessAadharCardNo { get; set; }

        public string BusinessAccountType { get; set; } = "Silver";  //  Silver, Gold, premium account

        public int OwnerProfileImageId { get; set; }

        public bool IsActivated { get; set; }
    }
}
