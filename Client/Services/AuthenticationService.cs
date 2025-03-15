using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Common;
using Models;

public class AuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public AuthenticationService(HttpClient httpClient, 
        ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
    }

    public async Task<AuthenticationResponseDTO> SignInAsync(CustomerLoginDTO loginDTO)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{SD.BaseApiUrl}/ClientSignIn", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadAsStringAsync();
            var authenticationResponse = JsonSerializer.Deserialize<AuthenticationResponseDTO>(authResponse);

            if (authenticationResponse.IsAuthSuccessful)
            {
                await _localStorageService.SetItemAsync("authToken", authenticationResponse.Token);
            }

            return authenticationResponse;
        }

        return null;
    }

    public async Task LogoutAsync()
    {
        await _localStorageService.RemoveItemAsync("authToken");
    }

    public async Task AddAuthorizationHeaderAsync(HttpClient client)
    {
        var token = await _localStorageService.GetItemAsync<string>("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
