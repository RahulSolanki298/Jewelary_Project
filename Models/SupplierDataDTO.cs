using System.Collections.Generic;

namespace Models
{
    public class SupplierDataDTO
    {
        public SupplierRegisterDTO SupplierInfo { get; set; } = new();

        public List<UserAddressDTO> SupplierAddress { get; set; } = new();

        public CompanyDataDTO CompanyInfo { get; set; } = new();

        public ChangePasswordDTO ChangePassword { get; set; } = new();

    }
}
