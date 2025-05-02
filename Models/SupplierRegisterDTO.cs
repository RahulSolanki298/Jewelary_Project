using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class SupplierRegisterDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="Please enter your first name.")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter your last name.")]
        public string LastName { get; set; }

        public string Gender { get; set; }

        public string EmailId { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your pancard no.")]
        public string PancardNo { get; set; }

        [Required(ErrorMessage = "Please enter your aadhar no.")]
        public string AadharCardNo { get; set; }

        public string TextPassword { get; set; }

        public bool IsBusinessAccount { get; set; } = false;

        public int? BusinessAccId { get; set; }

        public string GstNumber { get; set; }

        public string ActivationStatus { get; set; }

        
    }
}
