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

        public async Task<DiamondAllDataDto> GetDiamondListByFilter(DiamondFilters diamondFilters, int pageNumber = 1, int pageSize = 10)
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

                var result = await response.Content.ReadFromJsonAsync<DiamondAllDataDto>();

                return result ?? new DiamondAllDataDto();
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

        public async Task<DiamondData> GetDiamondById(int diamondId)
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/diamond/GetDiamond/diamondId/{diamondId}";

                var response = await _httpClient.GetFromJsonAsync<DiamondData>(requestUrl);

                
                if (response == null)
                {
                    throw new Exception("API response is null.");
                }

                return response;
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching diamond data: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<DiamondData>> GetSelectedDiamondByIds(int[] diamondIds)
        {
            try
            {
                // Build the request URL with the diamondIds query parameter
                var requestUrl = $"{SD.BaseApiUrl}/api/diamond/getDiamondListBydiamondIds";
                using var response = await _httpClient.PostAsJsonAsync(requestUrl, diamondIds);


                // Ensure the response is successful
                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                // Deserialize the response content into a List of DiamondData objects
                var result = await response.Content.ReadFromJsonAsync<List<DiamondData>>();

                return result ?? new List<DiamondData>(); // Return the result or an empty list if null
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching diamond data: {ex.Message}", ex);
            }
        }
        
        public async Task<IEnumerable<DiamondShapeData>> GetShapeListAsync()
        {
            try
            {
                //Get-Collection-List
                var requestUrl = $"{SD.BaseApiUrl}/api/diamondproperty/get-shape-list";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<DiamondShapeData>>();

                return result ?? new List<DiamondShapeData>();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching sub categories: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<DiamondColorData>> GetColorListAsync()
        {
            try
            {
                //Get-Collection-List
                var requestUrl = $"{SD.BaseApiUrl}/api/diamondproperty/get-color-list";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<DiamondColorData>>();

                return result ?? new List<DiamondColorData>();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching sub categories: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetCaratListAsync()
        {
            try
            {
                //Get-Collection-List
                var requestUrl = $"{SD.BaseApiUrl}/api/diamondproperty/get-carat-list";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<DiamondPropertyDTO>>();

                return result ?? new List<DiamondPropertyDTO>();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching sub categories: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetCutListAsync()
        {
            try
            {
                //Get-Collection-List
                var requestUrl = $"{SD.BaseApiUrl}/api/diamondproperty/get-cut-list";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<DiamondPropertyDTO>>();

                return result ?? new List<DiamondPropertyDTO>();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching sub categories: {ex.Message}", ex);
            }
        }

        public async Task<TableRangeDTO> GetTableRangesAsync()
        {
            try
            {
                //Get-Collection-List
                var requestUrl = $"{SD.BaseApiUrl}/api/diamondproperty/get-table-ranges";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<TableRangeDTO>();

                return result ?? new TableRangeDTO();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching sub categories: {ex.Message}", ex);
            }
        }

        public async Task<DepthDTO> GetDepthRangesAsync()
        {
            try
            {
                //Get-Collection-List
                var requestUrl = $"{SD.BaseApiUrl}/api/diamondproperty/get-depth-ranges";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<DepthDTO>();

                return result ?? new DepthDTO();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching sub categories: {ex.Message}", ex);
            }
        }

        public async Task<PriceRanges> GetProductPriceRangeData()
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/diamondProperty/get-price-ranges";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<PriceRanges>();

                return result ?? new PriceRanges();
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
