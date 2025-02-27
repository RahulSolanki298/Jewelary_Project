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
                AadharCardNo = customerRegister.AadharCardNo,
                PancardNo = customerRegister.PancardNo,
                TextPassword = customerRegister.TextPassword,
                Email = customerRegister.EmailId,
                UserName = customerRegister.EmailId,
                ActivationStatus = customerRegister.ActivationStatus,
                Gender=customerRegister.Gender,
                IsBusinessAccount=false,
                PhoneNumber=customerRegister.PhoneNumber                
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
                    AadharCardNo = customerRegister.Customer.AadharCardNo,
                    PancardNo = customerRegister.Customer.PancardNo,
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
                user.AadharCardNo = customerRegister.Customer.AadharCardNo;
                user.PancardNo = customerRegister.Customer.PancardNo;
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

        public async Task<IdentityResult> EmployeeRegisterAsync(CustomerRegisterDTO customerRegister)
        {
            var user = new ApplicationUser
            {
                FirstName = customerRegister.FirstName,
                MiddleName = customerRegister.MiddleName,
                LastName = customerRegister.LastName,
                AadharCardNo = customerRegister.AadharCardNo,
                PancardNo = customerRegister.PancardNo,
                TextPassword = customerRegister.TextPassword,
                Email = customerRegister.EmailId,
                UserName = customerRegister.EmailId,
                ActivationStatus = customerRegister.ActivationStatus,
                Gender = customerRegister.Gender,
                IsBusinessAccount = false,
                PhoneNumber = customerRegister.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, customerRegister.TextPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Employee);
            }

            return result;
        }

        public async Task<IdentityResult> SupplierRegisterAsync(CustomerRegisterDTO customerRegister)
        {
            var user = new ApplicationUser
            {
                FirstName = customerRegister.FirstName,
                MiddleName = customerRegister.MiddleName,
                LastName = customerRegister.LastName,
                AadharCardNo = customerRegister.AadharCardNo,
                PancardNo = customerRegister.PancardNo,
                TextPassword = customerRegister.TextPassword,
                Email = customerRegister.EmailId,
                UserName = customerRegister.EmailId,
                ActivationStatus = customerRegister.ActivationStatus,
                Gender = customerRegister.Gender,
                IsBusinessAccount = false,
                PhoneNumber = customerRegister.PhoneNumber                
            };

            var result = await _userManager.CreateAsync(user, customerRegister.TextPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Supplier);
            }

            return result;
        }


        public async Task<List<ApplicationUser>> GetCustomerData()
        {
           var resultDT= await _userManager.GetUsersInRoleAsync("Customer");
            return resultDT.ToList();
        }

        public async Task<List<ApplicationUser>> GetSuppliersData()
        {
            var resultDT = await _userManager.GetUsersInRoleAsync("Supplier");
            return resultDT.ToList();
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
