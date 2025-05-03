using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdminDashboard.Server.Service.IService;
using Blazored.LocalStorage;
using Common;
using Microsoft.AspNetCore.Components.Authorization;
using Models;

namespace AdminDashboard.Server.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<AuthenticationResponseDTO> AdminSignInAsync(AdminLoginModel loginDTO)
        {
            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{SD.BaseApiUrl}/api/account/admin-sign-in", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await JsonSerializer.DeserializeAsync<AuthenticationResponseDTO>(await response.Content.ReadAsStreamAsync());

                    if (authResponse.IsAuthSuccessful)
                    {
                        await _localStorage.SetItemAsync("authToken", authResponse.Token);
                        ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(authResponse.Token);
                    }

                    return authResponse;
                }
                else
                {
                    return new AuthenticationResponseDTO
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "Invalid credentials."
                    };
                }

            }
            catch (System.Exception)
            {

                throw;
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
