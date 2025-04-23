using Common;
using Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using DataAccess.Entities;
using System.Net.Http.Json;
using B2C_ECommerce.IServices;
using Microsoft.EntityFrameworkCore;

namespace B2C_ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<List<ProductDTO>> GetProductListByFilter(ProductFilters productFilters, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/product/GetProductsByFilters?pageNumber={pageNumber}&pageSize={pageSize}";

                // Use PostAsJsonAsync for serialization and sending the request
                using var response = await _httpClient.PostAsJsonAsync(requestUrl, productFilters);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                // Deserialize the response content directly
                var result = await response.Content.ReadFromJsonAsync<List<ProductDTO>>();

                return result ?? new List<ProductDTO>();
            }
            catch (HttpRequestException httpEx)
            {
                // Log or handle HTTP request-specific errors
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                // Log or handle general errors
                throw new Exception($"Error fetching product list: {ex.Message}", ex);
            }
        }

        public async Task<ProductDTO> GetProductByProductId(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                    throw new ArgumentException("Product ID cannot be null or empty.", nameof(productId));

                var requestUrl = $"{SD.BaseApiUrl}/api/product/GetProductDetails/{productId}";

                using var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var product = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return product ?? new ProductDTO();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error while fetching product {productId}: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching product with ID {productId}: {ex.Message}", ex);
            }
        }



        public async Task<List<ProductPropertyDTO>> GetProductColorList()
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/productFilters/get-color-list";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<ProductPropertyDTO>>();

                return result ?? new List<ProductPropertyDTO>();
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

        public async Task<List<CategoryDTO>> GetCategoriesList()
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/productFilters/Get-Category-List";


                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();

                return result ?? new List<CategoryDTO>();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"HTTP request error: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching categories: {ex.Message}", ex);
            }
        }

        public async Task<List<SubCategoryDTO>> GetSubcategoryList()
        {
            try
            {
                //Get-Collection-List
                var requestUrl = $"{SD.BaseApiUrl}/api/productFilters/Get-SubCat-List";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<SubCategoryDTO>>();

                return result ?? new List<SubCategoryDTO>();
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

        public async Task<List<ProductPropertyDTO>> GetShapeList()
        {
            try
            {
                //Get-Collection-List
                var requestUrl = $"{SD.BaseApiUrl}/api/productFilters/get-shape-list";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<ProductPropertyDTO>>();

                return result ?? new List<ProductPropertyDTO>();
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
                var requestUrl = $"{SD.BaseApiUrl}/api/productFilters/get-price-ranges";

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

        public async Task<ProductDTO> GetProductsByColorId(string sku, int? colorId = 0)
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/Product/GetProductByColor/Sku/{sku}/colorId/{colorId}";

                using var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return result ?? new ProductDTO();
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

        public async Task<ProductDTO> GetProductsByCaratId(string sku, int? caratId = 0)
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/Product/GetProductByCarat/Sku/{sku}/caratId/{caratId}";

                using var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return result ?? new ProductDTO();
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
