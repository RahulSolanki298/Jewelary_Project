using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Business.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountRepository(ApplicationDBContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IdentityResult> CustomerRegisterAsync(CustomerRegisterDTO customerRegister)
        {
            var user = new ApplicationUser
            {
                FirstName = customerRegister.FirstName,
                MiddleName = customerRegister.MiddleName,
                LastName = customerRegister.LastName,
                TextPassword = customerRegister.TextPassword,
                Email = customerRegister.EmailId,
                UserName = customerRegister.EmailId,
                ActivationStatus = customerRegister.ActivationStatus,
                Gender = customerRegister.Gender,
                IsBusinessAccount = false,
                PhoneNumber = customerRegister.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, customerRegister.TextPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Customer);
            }

            return result;
        }

        public async Task<IdentityResult> UpsertBusinessAccountAsync(BusinessAccountRegisterDTO customerRegister)
        {
            var existingBusinessAccount = await _context.BusinessAccount
                .FirstOrDefaultAsync(b => b.BusinessPanCardNo == customerRegister.BusinessPanCardNo);

            if (existingBusinessAccount == null)
            {
                existingBusinessAccount = new BusinessAccount
                {
                    AddressLine1 = customerRegister.AddressLine1,
                    AddressLine2 = customerRegister.AddressLine2,
                    BusinessAadharCardNo = customerRegister.BusinessAadharCardNo,
                    BusinessPanCardNo = customerRegister.BusinessPanCardNo,
                    BusinessAccountType = customerRegister.BusinessAccountType,
                    BusinessCertificate = customerRegister.BusinessCertificate,
                    BusinessEmailId = customerRegister.BusinessEmailId,
                    City = customerRegister.City,
                    CompanyName = customerRegister.CompanyName,
                    ContactNumber = customerRegister.ContactNumber,
                    LongDescription = customerRegister.LongDescription,
                    ShortDescription = customerRegister.ShortDescription,
                    Country = customerRegister.Country,
                    OfficialWebsite = customerRegister.OfficialWebsite,
                    ZipCode = customerRegister.ZipCode,
                    WhatsAppNumber = customerRegister.WhatsAppNumber,
                    State = customerRegister.State,
                    RegisterDate = customerRegister.RegisterDate,
                    IsActivated = customerRegister.IsActivated,
                    OwnerProfileImageId = customerRegister.OwnerProfileImageId,
                    CompanyLogoId = customerRegister.CompanyLogoId,
                    BusinessCode = GenerateBusinessAccountCode() // New Code generation
                };

                await _context.BusinessAccount.AddAsync(existingBusinessAccount);
            }
            else // If the business account exists, update the existing one
            {
                // Update properties if necessary
                existingBusinessAccount.AddressLine1 = customerRegister.AddressLine1;
                existingBusinessAccount.AddressLine2 = customerRegister.AddressLine2;
                existingBusinessAccount.BusinessAadharCardNo = customerRegister.BusinessAadharCardNo;
                existingBusinessAccount.BusinessPanCardNo = customerRegister.BusinessPanCardNo;
                existingBusinessAccount.ContactNumber = customerRegister.ContactNumber;
                existingBusinessAccount.City = customerRegister.City;
                existingBusinessAccount.CompanyName = customerRegister.CompanyName;
                existingBusinessAccount.ShortDescription = customerRegister.ShortDescription;
                existingBusinessAccount.LongDescription = customerRegister.LongDescription;
                existingBusinessAccount.Country = customerRegister.Country;
                existingBusinessAccount.ZipCode = customerRegister.ZipCode;
                existingBusinessAccount.WhatsAppNumber = customerRegister.WhatsAppNumber;
                existingBusinessAccount.State = customerRegister.State;
                existingBusinessAccount.RegisterDate = customerRegister.RegisterDate;
                existingBusinessAccount.IsActivated = customerRegister.IsActivated;
                existingBusinessAccount.OwnerProfileImageId = customerRegister.OwnerProfileImageId;
                existingBusinessAccount.CompanyLogoId = customerRegister.CompanyLogoId;

                _context.BusinessAccount.Update(existingBusinessAccount);
            }

            await _context.SaveChangesAsync();

            var user = await _userManager.FindByEmailAsync(customerRegister.Customer.EmailId);

            if (user == null) // If user doesn't exist, create new one
            {
                user = new ApplicationUser
                {
                    FirstName = customerRegister.Customer.FirstName,
                    MiddleName = customerRegister.Customer.MiddleName,
                    LastName = customerRegister.Customer.LastName,
                    TextPassword = customerRegister.Customer.TextPassword,
                    Email = customerRegister.Customer.EmailId,
                    UserName = customerRegister.Customer.EmailId,
                    ActivationStatus = customerRegister.Customer.ActivationStatus,
                    Gender = customerRegister.Customer.Gender,
                    IsBusinessAccount = true,
                    BusinessAccId = existingBusinessAccount.Id
                };

                var result = await _userManager.CreateAsync(user, customerRegister.Customer.TextPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, SD.BusinessAccount);
                }
                return result;
            }
            else
            {
                user.FirstName = customerRegister.Customer.FirstName;
                user.MiddleName = customerRegister.Customer.MiddleName;
                user.LastName = customerRegister.Customer.LastName;
                user.TextPassword = customerRegister.Customer.TextPassword;
                user.ActivationStatus = customerRegister.Customer.ActivationStatus;
                user.Gender = customerRegister.Customer.Gender;
                user.PhoneNumber = customerRegister.Customer.PhoneNumber;
                user.Email = customerRegister.Customer.EmailId;
                user.BusinessAccId = existingBusinessAccount.Id;
                user.IsBusinessAccount = true;
                user.ActivationStatus = customerRegister.Customer.ActivationStatus;

                var result = await _userManager.UpdateAsync(user);
                return result;
            }
        }

        public async Task<IdentityResult> EmployeeRegisterAsync(EmployeeRegisterDTO employeeRegister)
        {
            var user = new ApplicationUser
            {
                FirstName = employeeRegister.FirstName,
                MiddleName = employeeRegister.MiddleName,
                LastName = employeeRegister.LastName,
                AadharCardNo = employeeRegister.AadharCardNo,
                PancardNo = employeeRegister.PancardNo,
                TextPassword = employeeRegister.TextPassword,
                Email = employeeRegister.EmailId,
                UserName = employeeRegister.EmailId,
                ActivationStatus = employeeRegister.ActivationStatus,
                Gender = employeeRegister.Gender,
                IsBusinessAccount = false,
                PhoneNumber = employeeRegister.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, employeeRegister.TextPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Employee);
            }

            return result;
        }

        public async Task<IdentityResult> SupplierRegisterAsync(SupplierRegisterDTO supplierRegister)
        {
            try
            {
                var user = new ApplicationUser
                {
                    FirstName = supplierRegister.FirstName,
                    MiddleName = supplierRegister.MiddleName,
                    LastName = supplierRegister.LastName,
                    AadharCardNo = supplierRegister.AadharCardNo,
                    PancardNo = supplierRegister.PancardNo,
                    TextPassword = supplierRegister.TextPassword,
                    Email = supplierRegister.EmailId,
                    UserName = supplierRegister.EmailId,
                    ActivationStatus = supplierRegister.ActivationStatus,
                    Gender = supplierRegister.Gender,
                    GstNumber=supplierRegister.GstNumber,
                    IsBusinessAccount = false,
                    PhoneNumber = supplierRegister.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, supplierRegister.TextPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, SD.Supplier);
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<List<ApplicationUser>> GetCustomerData()
        {
            var resultDT = await _userManager.GetUsersInRoleAsync("Customer");
            return resultDT.ToList();
        }

        public async Task<List<ApplicationUser>> GetSuppliersData()
        {
            var resultDT = await _userManager.GetUsersInRoleAsync("Supplier");
            return resultDT.ToList();
        }

        public async Task<SupplierDataDTO> GetSupplierAllData(string supplierId)
        {
            var data = new SupplierDataDTO();
            data.SupplierInfo = new SupplierRegisterDTO();
            data.SupplierAddress = new List<UserAddressDTO>();
            data.SupplierInfo = await (from ausr in _context.ApplicationUser
                                       where ausr.Id == supplierId
                                       select new SupplierRegisterDTO
                                       {
                                           Id = ausr.Id,
                                           FirstName = ausr.FirstName,
                                           MiddleName = ausr.MiddleName,
                                           LastName = ausr.LastName,
                                           EmailId = ausr.Email,
                                           PhoneNumber = ausr.PhoneNumber,
                                           AadharCardNo = ausr.AadharCardNo,
                                           Gender = ausr.Gender,
                                           PancardNo = ausr.PancardNo,
                                           ActivationStatus = ausr.ActivationStatus,
                                           TextPassword = ausr.TextPassword
                                       }).FirstOrDefaultAsync();

            data.CompanyInfo = await (from comp in _context.CompanyData
                                      join ausr in _context.ApplicationUser on comp.VendarId equals ausr.Id
                                      where comp.VendarId == supplierId
                                       select new CompanyDataDTO
                                       {
                                           Id = comp.Id,
                                           CompanyLogo = comp.CompanyLogo,
                                           CompanyName = comp.CompanyName,
                                           AddressLine1 = comp.AddressLine1,
                                           AddressLine2 = comp.AddressLine2,
                                           CityName = comp.CityName,
                                           CountryName = comp.CountryName,
                                           EmailId = comp.EmailId,
                                           Registration_Number = comp.Registration_Number,
                                           Description = comp.Description,
                                           Founded_Date = comp.Founded_Date,
                                           VendarId = comp.VendarId,
                                           ZipCode = comp.ZipCode,
                                           Website = comp.Website,
                                           StateName=comp.StateName,
                                           PhoneNo1=comp.PhoneNo1,
                                           PhoneNo2=comp.PhoneNo2,
                                           CreatedBy=comp.CreatedBy,
                                           UpdatedBy=comp.UpdatedBy
                                       }).FirstOrDefaultAsync();

            data.SupplierAddress = await (from adr in _context.UserAddress 
                                          join ausr in _context.ApplicationUser on adr.UserId equals ausr.Id
                                          where adr.UserId == supplierId
                                          select new UserAddressDTO
                                          {
                                              Id=adr.Id,
                                              UserId=ausr.Id,
                                              AddressLine1=adr.AddressLine1,
                                              AddressLine2=adr.AddressLine2,
                                              CityName=adr.CityName,
                                              StateName=adr.StateName,
                                              Location=adr.Location,
                                              Pincode=adr.Pincode,
                                              IsDefaultAddress=adr.IsDefaultAddress
                                          }).ToListAsync();

            data.ChangePassword = new ChangePasswordDTO();

            return data;
        }

        public async Task<List<ApplicationUser>> GetBusinessAccountData()
        {
            var resultDT = await _userManager.GetUsersInRoleAsync("BusinessAccount");
            return resultDT.ToList();
        }

        public async Task<List<ApplicationUser>> GetEmployeeList()
        {
            var resultDT = await _userManager.GetUsersInRoleAsync("Employee");
            return resultDT.ToList();
        }

        private string GenerateBusinessAccountCode()
        {
            string currentDate = DateTime.Now.ToString("yyyyMMdd");

            Random rand = new Random();
            int uniqueCode = rand.Next(10000, 100000); // 5-digit number

            string accountCode = $"GJ_{currentDate}_{uniqueCode}";

            return accountCode;
        }

    }

}
