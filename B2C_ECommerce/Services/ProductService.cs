﻿using Common;
using Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using DataAccess.Entities;
using System.Net.Http.Json;
using B2C_ECommerce.IServices;

namespace B2C_ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<List<Product>> GetProductListByFilter()
        {
            try
            {
                var requestUrl = $"{SD.BaseApiUrl}/api/product/GetProductCollectionNewList";

                using var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws exception if status code is not successful.

                if (response.Content is null)
                {
                    throw new Exception("API response content is null.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<Product>>();

                return result ?? new List<Product>();
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
