using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CustomerLoginDTO
    {
        [Required(ErrorMessage ="Please enter username.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        public string Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
