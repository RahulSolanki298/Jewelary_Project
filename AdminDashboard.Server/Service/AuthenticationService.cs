using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdminDashboard.Server.Service.IService;
using Blazored.LocalStorage;
using Common;
using DataAccess.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Models;

namespace AdminDashboard.Server.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationService(HttpClient httpClient,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage, SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<AuthenticationResponseDTO> AdminSignInAsync(AdminLoginModel loginDTO)
        {
            try
            {
                // Step 1: Find the user
                var user = await _userManager.FindByNameAsync(loginDTO.UserName);
                if (user == null)
                {
                    return new AuthenticationResponseDTO
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "User not found."
                    };
                }

                // Step 2: Verify the password
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (!isPasswordValid)
                {
                    return new AuthenticationResponseDTO
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "Invalid credentials."
                    };
                }

                // Step 3: Get roles
                var roles = await _userManager.GetRolesAsync(user);
                if (roles == null || !roles.Any())
                {
                    return new AuthenticationResponseDTO
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "User does not have assigned roles."
                    };
                }

                // Step 4: Return successful login response
                return new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = true,
                    Token = null, // Add JWT here if needed
                    Roles = roles.ToList(),
                    userDTO = new UserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = roles.ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                // Optional: Log exception
                return new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "An error occurred during sign-in."
                };
            }
        }




        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            return !string.IsNullOrEmpty(token);
        }
    }
}
