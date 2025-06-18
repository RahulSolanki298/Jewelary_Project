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
                    ErrorMessage = "Invalid username or password."
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDTO.Password, loginDTO.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = true,
                    SuccessMessage = "Login successful."
                };
            }
            else if (result.IsLockedOut)
            {
                return new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Account is locked out."
                };
            }
            else if (result.IsNotAllowed)
            {
                return new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Login not allowed. Please confirm your email."
                };
            }

            return new AuthenticationResponseDTO
            {
                IsAuthSuccessful = false,
                ErrorMessage = "Invalid username or password."
            };
        }

        // Example for Supplier Sign-Up
        public async Task<RegisterationResponseDTO> SupplierSignUpAsync(UserRequestDTO userRequestDTO)
        {
            var jsonContent = JsonConvert.SerializeObject(userRequestDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/account/supplier-sign-up", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RegisterationResponseDTO>(result);
            }
            return null;
        }


        // Customer Sign-Up method
        public async Task<ApplicationUser> CustomerSignUpAsync(UserRequestDTO userRequestDTO)
        {
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
