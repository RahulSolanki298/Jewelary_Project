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
using Business.Repository.IRepository;
using DataAccess.Data;
using System.Linq;
using System.Drawing;

namespace B2C_ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDBContext _context;
        private readonly IProductPropertyRepository _productPropRepository;
        private readonly IProductRepository _productRepository;
        public ProductService(IHttpClientFactory httpClientFactory,
            ApplicationDBContext context,
            IProductPropertyRepository productPropertyRepository,
            IProductRepository productRepository)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _context = context;
            _productPropRepository = productPropertyRepository;
            _productRepository = productRepository;
        }

        public async Task<List<ProductDTO>> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10)
        {
            var products = await _productRepository.GetProductStyleList();
            var query = products.AsQueryable();

            var shapeIds = filters.Shapes?.Select(Int32.Parse).ToList();
            var metalIds = filters.Metals?.Select(Int32.Parse).ToList();

            if (shapeIds?.Any() == true)
            {
                query = query.Where(p => p.Shapes.Any(shape => shapeIds.Contains(p.ShapeId.Value)));
            }

            if (metalIds?.Any() == true)
            {
                query = query.Where(p => p.Metals.Any(metal => metalIds.Contains(p.ColorId.Value)));
            }

            if (filters.FromPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filters.FromPrice.Value);
            }

            if (filters.ToPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filters.ToPrice.Value);
            }

            if (filters.FromCarat.HasValue)
            {
                query = query.Where(p => Convert.ToDecimal(p.CenterCaratName) >= filters.FromCarat.Value);
            }

            if (filters.ToCarat.HasValue)
            {
                query = query.Where(p => Convert.ToDecimal(p.CenterCaratName) <= filters.ToCarat.Value);
            }

            if (filters.categories != null && filters.categories.Length > 0 && filters.categories[0] != null)
            {
                query = query.Where(p => filters.categories.Contains(p.CategoryName));
            }

            if (!string.IsNullOrEmpty(filters.OrderBy))
            {
                switch (filters.OrderBy.ToLower())
                {
                    case "asc":
                        query = query.OrderBy(p => p.Title); // assuming product has a Name property
                        break;
                    case "desc":
                        query = query.OrderByDescending(p => p.Title);
                        break;
                    case "price":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "priceMax":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                }
            }

            // Pagination
            var pagedResult = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return pagedResult;
        }

        public async Task<ProductDTO> GetProductByProductId(string productId)
        {
            var result = await _productRepository.GetProductWithDetails(productId);
            return result;
        }



        public async Task<IEnumerable<ProductPropertyDTO>> GetProductColorList()
        {
            try
            {

                var colors = await _productPropRepository.GetProductColorList();
                return colors;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<CategoryDTO>> GetCategoriesList()
        {
            var categories = await (from prd in _context.Category
                                    select new CategoryDTO
                                    {
                                        Id = prd.Id,
                                        Name = prd.Name,
                                    }).ToListAsync();

            return categories;
        }

        public async Task<List<SubCategoryDTO>> GetSubcategoryList()
        {
            var categories = await (from prd in _context.SubCategory
                                    select new SubCategoryDTO
                                    {
                                        Id = prd.Id,
                                        Name = prd.Name,
                                    }).ToListAsync();

            return categories;
        }

        public async Task<IEnumerable<ProductPropertyDTO>> GetShapeList()
        {
            return await _productPropRepository.GetProductShapeList();
        }

        public async Task<PriceRanges> GetProductPriceRangeData()
        {
            var data = await _context.Product.ToListAsync();
            var priceRange = new PriceRanges();
            priceRange.MaxPrice = data.Max(x => x.Price);
            priceRange.MinPrice = data.Min(x => x.Price);

            return priceRange;
        }

        public async Task<ProductDTO> GetProductsByColorId(string sku, int? colorId = 0)
        {
            var products = await _productRepository.GetProductByColorId(sku, colorId, 0);
            return products;
        }

        public async Task<ProductDTO> GetProductsByCaratId(string sku, int? caratId = 0)
        {
            var products = await _productRepository.GetProductByColorId(sku, 0, caratId);
            return products;
        }

    }
}
