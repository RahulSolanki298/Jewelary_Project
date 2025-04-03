using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using B2C_ECommerce.IServices;
using Common;
using Models;

namespace B2C_ECommerce.Services
{
    public class DiamondService : IDiamondService
    {
        private readonly HttpClient _httpClient;

        public DiamondService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<List<DiamondData>> GetDiamondListByFilter(DiamondFilters diamondFilters, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/diamond/GetDiamondList?pageNumber={pageNumber}&pageSize={pageSize}";

                using var response = await _httpClient.PostAsJsonAsync(requestUrl, diamondFilters);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<DiamondData>>();

                return result ?? new List<DiamondData>();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching diamond list: {ex.Message}", ex);
            }
        }


    }
}
