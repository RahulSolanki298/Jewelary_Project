using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CustomerRegisterAsync(CustomerRegisterDTO customerRegister);

        Task<IdentityResult> EmployeeRegisterAsync(EmployeeRegisterDTO employeeRegister);

        Task<IdentityResult> SupplierRegisterAsync(SupplierRegisterDTO supplierRegister);

        Task<IdentityResult> UpsertBusinessAccountAsync(BusinessAccountRegisterDTO customerRegister);

        Task<List<ApplicationUser>> GetCustomerData();

        Task<List<ApplicationUser>> GetSuppliersData();

        Task<List<ApplicationUser>> GetBusinessAccountData();

        Task<List<ApplicationUser>> GetEmployeeList();

        Task<SupplierDataDTO> GetSupplierData(string supplierId);
    }
}
