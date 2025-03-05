using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string VenderGroupName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string PancardNo { get; set; }

        public string AadharCardNo { get; set; }

        public string TextPassword { get; set; }

        public bool IsBusinessAccount { get; set; } = false;

        public int? BusinessAccId { get; set; }

        public bool IsCustomer { get; set; } = false;

        public string CustomerCode { get; set; }

        public string ActivationStatus { get; set; }
    }
}
