using System.Collections.Generic;

namespace Models
{
    public class SupplierDataDTO
    {
        public  SupplierRegisterDTO SupplierInfo { get; set; }

        public List<UserAddressDTO> SupplierAddress { get; set; }

        public CompanyDataDTO CompanyInfo { get; set; }

        public ChangePasswordDTO ChangePassword { get; set; }

    }
}
