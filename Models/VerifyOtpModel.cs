using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VerifyOtpModel
    {
        [Required]
        [Display(Name = "OTP Code")]
        public string OtpCode { get; set; }

        public string UserId { get; set; }
    }
}
