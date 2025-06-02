using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AdminLoginModel
    {
        [Required(ErrorMessage ="Please eneter username.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please eneter password.")]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
