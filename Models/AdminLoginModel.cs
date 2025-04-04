using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AdminLoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
