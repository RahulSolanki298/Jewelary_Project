using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CustomerRegisterDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage ="Please enter your first name.")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter your last name.")]
        public string LastName { get; set; }

        public string Gender { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string EmailId { get; set; }


        [Required(ErrorMessage = "Please enter a password.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string TextPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("TextPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public bool IsBusinessAccount { get; set; } = false;

        public string ActivationStatus { get; set; }
        
    }
}
