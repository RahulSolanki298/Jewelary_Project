using B2C_ECommerce.IServices;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace B2C_ECommerce.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDBContext _context;
        public AccountService(IHttpClientFactory httpClientFactory, UserManager<ApplicationUser> userManager, ApplicationDBContext context, SignInManager<ApplicationUser> signInManager)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }


        // Customer Sign-In method
        public async Task<AuthenticationResponseDTO> CustomerSignInAsync(CustomerLoginDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.Username);

            if (user == null)
            {
                return new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Authentication unsuccessful. Check your username and password."
                };
            }

            if (user.ActivationStatus != SD.Activated)
            {
                return new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Login restricted: Your account is currently inactive. Contact support for assistance."
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDTO.Password, loginDTO.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = true,
                    SuccessMessage = "Success: Your account has been created."
                };
            }

            string errorMessage = result.IsLockedOut
                ? "Your account has been locked. Please try again later or contact support."
                : result.IsNotAllowed
                    ? "Login not permitted. Please verify your email address to proceed."
                    : "Incorrect username or password.";

            return new AuthenticationResponseDTO
            {
                IsAuthSuccessful = false,
                ErrorMessage = errorMessage,
                userDTO = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNo = user.PhoneNumber,
                    Email = user.Email
                }
            };
        }


        // Example for Supplier Sign-Up
        public async Task<ApplicationUser> SupplierSignUpAsync(SupplierRegisterDTO userRequestDTO)
        {
            // Check if the email is already registered
            var existingUser = await _userManager.FindByEmailAsync(userRequestDTO.EmailId);
            if (existingUser != null)
            {
                throw new Exception("Email already in use. Try logging in or choose another address.");
            }

            var user = new ApplicationUser()
            {
                UserName = userRequestDTO.EmailId,
                Email = userRequestDTO.EmailId,
                FirstName = userRequestDTO.FirstName,
                LastName = userRequestDTO.LastName,
                PhoneNumber = userRequestDTO.PhoneNumber,
                EmailConfirmed = true,
                IsBusinessAccount=true,
                ActivationStatus=SD.Pending,
                TextPassword=userRequestDTO.TextPassword,
                AadharCardNo=userRequestDTO.AadharCardNo,
                PancardNo=userRequestDTO.PancardNo,
                GstNumber=userRequestDTO.GstNumber,
                MiddleName=userRequestDTO.MiddleName,
            };

            var result = await _userManager.CreateAsync(user, userRequestDTO.TextPassword);

            if (!result.Succeeded)
            {
                throw new Exception("User creation failed: ");
            }

            await _userManager.AddToRoleAsync(user, SD.Supplier); // Assuming SD.Supplier = "Supplier"
            return user;
        }



        // Customer Sign-Up method
        public async Task<ApplicationUser> CustomerSignUpAsync(UserRequestDTO userRequestDTO)
        {
            // Check if the email is already registered
            var existingUser = await _userManager.FindByEmailAsync(userRequestDTO.Email);
            if (existingUser != null)
            {
                // You can throw an exception or handle it as per your requirement
                throw new InvalidOperationException("Email is already registered.");
            }

            var user = new ApplicationUser()
            {
                UserName = userRequestDTO.Email,
                Email = userRequestDTO.Email,
                FirstName = userRequestDTO.FirstName,
                LastName = userRequestDTO.LastName,
                PhoneNumber = userRequestDTO.PhoneNo,
                IsCustomer = true,
                CustomerCode = await GenerateCustomerCode(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userRequestDTO.Password);

            if (!result.Succeeded)
            {
                // You might want to throw an error or return null / result.Errors
                throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, SD.Customer);
            return user;
        }


        private async Task<string> GenerateCustomerCode()
        {
            // Simulate async work, e.g., database or remote service call
            await Task.Delay(10);

            string prefix = "JF";
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string randomDigits = new Random().Next(1000, 9999).ToString();

            string customerCode = $"{prefix}-{timestamp}-{randomDigits}";
            return customerCode;
        }

        
    }
}
