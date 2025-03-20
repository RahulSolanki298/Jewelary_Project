using B2C_ECommerce.IServices;
using Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace B2C_ECommerce.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        // Customer Sign-Up method
        public async Task<RegisterationResponseDTO> CustomerSignUpAsync(UserRequestDTO userRequestDTO)
        {
            var jsonContent = JsonConvert.SerializeObject(userRequestDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/account/customer-sign-up", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RegisterationResponseDTO>(result);
            }
            return null;
        }

        // Customer Sign-In method
        public async Task<AuthenticationResponseDTO> CustomerSignInAsync(CustomerLoginDTO loginDTO)
        {
            var jsonContent = JsonConvert.SerializeObject(loginDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/account/customer-sign-in", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AuthenticationResponseDTO>(result);
            }
            return null;
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
    }
}
