using B2C_ECommerce.IServices;
using Common;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace B2C_ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private ApplicationDBContext _context;
        public ProductService(IHttpClientFactory httpClientFactory,
            ApplicationDBContext context)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _context = context;
        }

        public async Task<List<ProductDTO>> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10)
        {
                var products = await GetProductStyleList();

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
            var pagedResult = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return pagedResult;
        }

        public async Task<ProductDTO> GetProductByProductId(string productId)
        {
            var productQuery = await (
                  from product in _context.Product
                  where product.IsActivated && product.Id.ToString() == productId
                  join evt in _context.EventSites on product.EventId equals evt.Id
                  join cat in _context.Category on product.CategoryId equals cat.Id

                  join colorProp in _context.ProductProperty on product.ColorId equals colorProp.Id into colorGroup
                  from color in colorGroup.DefaultIfEmpty()

                  join shapeProp in _context.ProductProperty on product.CenterShapeId equals shapeProp.Id into shapeGroup
                  from shape in shapeGroup.DefaultIfEmpty()

                  join ashapeProp in _context.ProductProperty on product.AccentStoneShapeId equals ashapeProp.Id into ashapeGroup
                  from ashape in ashapeGroup.DefaultIfEmpty()

                  join clarityProp in _context.ProductProperty on product.ClarityId equals clarityProp.Id into clarityGroup
                  from clarity in clarityGroup.DefaultIfEmpty()

                  join sizeProp in _context.ProductProperty on product.CenterCaratId equals sizeProp.Id into sizeGroup
                  from size in sizeGroup.DefaultIfEmpty()

                  select new ProductDTO
                  {
                      Id = product.Id,
                      Title = evt.EventName,
                      EventName = evt.EventName,
                      EventId = product.EventId,
                      BandWidth = product.BandWidth,
                      Length = product.Length,
                      CaratName = product.Carat,
                      CategoryId = cat.Id,
                      CategoryName = cat.Name,
                      ProductType = cat.ProductType,
                      ColorId = product.ColorId,
                      ColorName = color.Name,
                      ClarityId = product.ClarityId,
                      ClarityName = clarity.Name,
                      CenterShapeId = product.CenterShapeId,
                      CenterShapeName = shape.Name,
                      AccentStoneShapeId = product.AccentStoneShapeId,
                      AccentStoneShapeName = ashape.Name,
                      CaratSizeId = product.CaratSizeId,
                      Description = product.Description,
                      Sku = product.Sku,
                      UnitPrice = product.UnitPrice,
                      Price = product.Price,
                      IsActivated = product.IsActivated
                  }
              ).AsNoTracking().FirstOrDefaultAsync();



            // Step 1: Group products by SKU

            var firstProduct = productQuery; // Get the first product from the group

            // Step 2: Get all related properties for each group
            var metals = await (from col in _context.ProductProperty
                                join prod in _context.Product on col.Id equals prod.ColorId
                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                where colN.Name == SD.Metal && prod.Sku == firstProduct.Sku
                                select new ProductPropertyDTO
                                {
                                    Id = col.Id,
                                    Name = col.Name,
                                    SymbolName = col.SymbolName,
                                    IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                }).Distinct().ToListAsync();

            var caratSizes = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.CenterCaratId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.CaratSize && prod.Sku == firstProduct.Sku
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                    }).Distinct().ToListAsync();

            var shapes = await (from col in _context.ProductProperty
                                join prod in _context.Product on col.Id equals prod.CenterShapeId
                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                where colN.Name == SD.Shape && prod.Sku == firstProduct.Sku
                                select new ProductPropertyDTO
                                {
                                    Id = col.Id,
                                    Name = col.Name,
                                    IconPath = col.IconPath,
                                    IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                }).Distinct().ToListAsync();


            // Step 3: Create a ProductDTO for the group and add related properties
            var productDTO = new ProductDTO
            {
                Id = firstProduct.Id,
                Title = firstProduct.Title,
                CaratName = firstProduct.CaratName,
                CategoryId = firstProduct.CategoryId,
                CategoryName = firstProduct.CategoryName,
                ColorId = firstProduct.ColorId,
                ColorName = firstProduct.ColorName,
                ClarityId = firstProduct.ClarityId,
                ClarityName = firstProduct.ClarityName,
                CenterShapeId = firstProduct.CenterShapeId,
                CenterShapeName = firstProduct.CenterShapeName,
                UnitPrice = firstProduct.UnitPrice,
                Price = firstProduct.Price,
                IsActivated = firstProduct.IsActivated,
                CenterCaratId = firstProduct.CenterCaratId,
                CenterCaratName = firstProduct.CenterCaratName,
                Description = firstProduct.Description,
                Sku = firstProduct.Sku,
                ProductType = firstProduct.ProductType,
                StyleId = firstProduct.StyleId,
                Metals = metals,
                CaratSizes = caratSizes,
                Shapes = shapes,
                BandWidth = firstProduct.BandWidth,
                Length = firstProduct.Length,
                ProductImageVideos = new List<ProductImageAndVideoDTO>() // Initialize to avoid null reference
            };

            // Step 4: Get the product images for the first product
            var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.Id.ToString()).ToListAsync();

            foreach (var image in productImages)
            {
                string imageUrl = "-";
                string videoUrl = "-";

                if (image.ImageLgId.HasValue)
                {
                    imageUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.ImageLgId.Value)?.FileUrl ?? "-";
                }
                else if (image.VideoId.HasValue)
                {
                    videoUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.VideoId.Value)?.FileUrl ?? "-";
                }

                var imageVideo = new ProductImageAndVideoDTO
                {
                    ImageUrl = imageUrl,
                    VideoUrl = videoUrl
                };

                productDTO.ProductImageVideos.Add(imageVideo);
            }



            // Return products where there are product images/videos
            return productDTO;
        }

        public async Task<List<ProductPropertyDTO>> GetProductColorList()
        {
            try
            {


                var colors = await (from prd in _context.ProductProperty
                                    join met in _context.ProductProperty on prd.ParentId equals met.Id
                                    where prd.IsActive == true && met.Name == SD.Metal
                                    select new ProductPropertyDTO
                                    {
                                        Id = prd.Id,
                                        Name = prd.Name,
                                        Description = prd.Description,
                                        SymbolName = prd.SymbolName,
                                        IconPath = prd.IconPath,
                                        ParentId = met.Id,
                                        ParentProperty = "-"
                                    }).ToListAsync();

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

        public async Task<List<ProductPropertyDTO>> GetShapeList()
        {
            try
            {
                var colors = await (from prd in _context.ProductProperty
                                    join shape in _context.ProductProperty on prd.ParentId equals shape.Id
                                    where prd.IsActive == true && shape.Name == SD.Shape
                                    select new ProductPropertyDTO
                                    {
                                        Id = prd.Id,
                                        Name = prd.Name,
                                        Description = prd.Description,
                                        SymbolName = prd.SymbolName,
                                        IconPath = prd.IconPath,
                                        ParentId = shape.Id,
                                        ParentProperty = "-"
                                    }).ToListAsync();

                return colors;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PriceRanges> GetProductPriceRangeData()
        {
            var priceRange = await _context.Product
                .Where(p => p.Price != null) // Optional: exclude null prices
                .GroupBy(_ => 1)
                .Select(g => new PriceRanges
                {
                    MinPrice = g.Min(x => x.Price),
                    MaxPrice = g.Max(x => x.Price)
                })
                .FirstOrDefaultAsync();

            return priceRange ?? new PriceRanges(); // Fallback to empty object if no products
        }


        public async Task<ProductDTO> GetProductsByColorId(string sku, int? colorId = 0, int? caratId = 0)
        {
            var products = await (from product in _context.Product
                                  join cat in _context.Category on product.CategoryId equals cat.Id
                                  join color in _context.ProductProperty on product.ColorId equals color.Id into colorGroup
                                  from color in colorGroup.DefaultIfEmpty()
                                  join shape in _context.ProductProperty on product.ShapeId equals shape.Id into shapeGroup
                                  from shape in shapeGroup.DefaultIfEmpty()
                                  join cen_shape in _context.ProductProperty on product.CenterShapeId equals cen_shape.Id into cen_shapeGroup
                                  from cen_shape in cen_shapeGroup.DefaultIfEmpty()
                                  join cen_crt in _context.ProductProperty on product.CenterCaratId equals cen_crt.Id into cen_CrtGroup
                                  from cen_crt in cen_CrtGroup.DefaultIfEmpty()
                                  join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityGroup
                                  from clarity in clarityGroup.DefaultIfEmpty()
                                  join size in _context.ProductProperty on product.CaratSizeId equals size.Id into sizeGroup
                                  from size in sizeGroup.DefaultIfEmpty()
                                  join kt in _context.ProductProperty on product.KaratId equals kt.Id into ktGroup
                                  from kt in ktGroup.DefaultIfEmpty()
                                  where product.IsActivated != false && product.Sku == sku
                                  select new ProductDTO
                                  {
                                      Id = product.Id,
                                      Title = product.Title,
                                      BandWidth = product.BandWidth,
                                      Length = product.Length,
                                      CaratName = product.Carat,
                                      CategoryId = cat.Id,
                                      CategoryName = cat.Name,
                                      ColorId = color != null ? color.Id : (int?)null,
                                      ColorName = color != null ? color.Name : null,
                                      ClarityId = clarity != null ? clarity.Id : (int?)null,
                                      ClarityName = clarity != null ? clarity.Name : null,
                                      ShapeName = shape != null ? shape.Name : null,
                                      ShapeId = shape != null ? shape.Id : (int?)null,
                                      CenterCaratId = cen_crt != null ? cen_crt.Id : (int?)null,
                                      CenterCaratName = cen_crt != null ? cen_crt.Name : null,
                                      CenterShapeId = cen_shape != null ? cen_shape.Id : (int?)null,
                                      CenterShapeName = cen_shape != null ? cen_shape.Name : null,
                                      UnitPrice = product.Price,
                                      Price = product.Price,
                                      IsActivated = product.IsActivated,
                                      CaratSizeId = product.CaratSizeId,
                                      Description = product.Description,
                                      Sku = product.Sku,
                                      ProductType = cat.ProductType,
                                      KaratId = product.KaratId,
                                      Karat = kt != null ? kt.Name : null
                                  }).ToListAsync();


            var query = products.AsQueryable();

            if (caratId.HasValue && caratId > 0)
            {
                query = query.Where(p => p.CenterCaratId == caratId);
            }

            if (colorId.HasValue && colorId > 0)
            {
                query = query.Where(p => p.ColorId == colorId);
            }

            var firstProduct = query.First();


            // Step 2: Get all related properties for each group
            var metals = await (from col in _context.ProductProperty
                                join prod in _context.Product on col.Id equals prod.ColorId
                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                where colN.Name == SD.Metal && prod.Sku == firstProduct.Sku
                                select new ProductPropertyDTO
                                {
                                    Id = col.Id,
                                    Name = col.Name,
                                    SymbolName = col.SymbolName,
                                    IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                }).Distinct().ToListAsync();

            var caratSizes = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.CenterCaratId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.CaratSize && prod.Sku == firstProduct.Sku
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                    }).Distinct().ToListAsync();

            var shapes = await (from col in _context.ProductProperty
                                join prod in _context.Product on col.Id equals prod.CenterShapeId
                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                where colN.Name == SD.Shape && prod.Sku == firstProduct.Sku
                                select new ProductPropertyDTO
                                {
                                    Id = col.Id,
                                    Name = col.Name,
                                    IconPath = col.IconPath,
                                    IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                }).Distinct().ToListAsync();


            // Step 3: Create a ProductDTO for the group and add related properties
            var productDTO = new ProductDTO
            {
                Id = firstProduct.Id,
                Title = firstProduct.Title,
                CaratName = firstProduct.CaratName,
                CategoryId = firstProduct.CategoryId,
                CategoryName = firstProduct.CategoryName,
                ColorId = firstProduct.ColorId,
                ColorName = firstProduct.ColorName,
                ClarityId = firstProduct.ClarityId,
                ClarityName = firstProduct.ClarityName,
                ShapeName = firstProduct.ShapeName,
                ShapeId = firstProduct.ShapeId,
                CenterShapeName = firstProduct.CenterShapeName,
                CenterShapeId = firstProduct.CenterShapeId,
                CenterCaratId = firstProduct.CenterCaratId,
                CenterCaratName = firstProduct.CenterCaratName,
                UnitPrice = firstProduct.UnitPrice,
                Price = firstProduct.Price,
                IsActivated = firstProduct.IsActivated,
                CaratSizeId = firstProduct.CaratSizeId,
                Description = firstProduct.Description,
                Sku = firstProduct.Sku,
                ProductType = firstProduct.ProductType,
                StyleId = firstProduct.StyleId,
                Metals = metals,
                CaratSizes = caratSizes,
                Shapes = shapes,
                BandWidth = firstProduct.BandWidth,
                Length = firstProduct.Length,
                KaratId = firstProduct.KaratId,
                Karat = firstProduct.Karat,
                ProductImageVideos = new List<ProductImageAndVideoDTO>() // Initialize to avoid null reference
            };

            // Step 4: Get the product images for the first product
            var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.Id.ToString() && x.MetalId == colorId).ToListAsync();

            foreach (var image in productImages)
            {
                var imageUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.ImageLgId)?.FileUrl ?? "-";
                var videoUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.VideoId)?.FileUrl ?? "-";

                var imageVideo = new ProductImageAndVideoDTO
                {
                    ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
                    VideoUrl = string.IsNullOrWhiteSpace(videoUrl) && videoUrl != null ? null : videoUrl
                };

                productDTO.ProductImageVideos.Add(imageVideo);
            }



            // Return products where there are product images/videos
            return productDTO;
        }

        private async Task<IEnumerable<ProductDTO>> GetProductStyleList()
        {
            var products = await (from product in _context.Product
                                  join evt in _context.EventSites on product.EventId equals evt.Id
                                  join krt in _context.ProductProperty on product.KaratId equals krt.Id
                                  join cat in _context.Category on product.CategoryId equals cat.Id
                                  join color in _context.ProductProperty on product.ColorId equals color.Id into colorGroup
                                  from color in colorGroup.DefaultIfEmpty()
                                  join shape in _context.ProductProperty on product.CenterShapeId equals shape.Id into shapeGroup
                                  from shape in shapeGroup.DefaultIfEmpty()
                                  join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityGroup
                                  from clarity in clarityGroup.DefaultIfEmpty()
                                  join size in _context.ProductProperty on product.CenterCaratId equals size.Id into sizeGroup
                                  from size in sizeGroup.DefaultIfEmpty()
                                  select new ProductDTO
                                  {
                                      Id = product.Id,
                                      Title = product.Title,
                                      EventId = product.EventId,
                                      EventName = evt.EventName,
                                      BandWidth = product.BandWidth,
                                      Length = product.Length,
                                      CaratName = product.Carat,
                                      CategoryId = cat != null ? cat.Id : (int?)null,
                                      CategoryName = cat.Name,
                                      ColorId = color != null ? color.Id : (int?)null,
                                      ColorName = color.Name,
                                      ClarityId = clarity != null ? clarity.Id : (int?)null,
                                      ClarityName = clarity.Name,
                                      ShapeName = shape.Name,
                                      //ShapeId = shape != null ? shape.Id : (int?)null,
                                      CenterShapeName = shape.Name,
                                      UnitPrice = product.UnitPrice,
                                      Price = product.Price,
                                      IsActivated = product.IsActivated,
                                      CaratSizeId = product.CaratSizeId,
                                      Description = product.Description,
                                      Sku = product.Sku,
                                      ProductType = cat.ProductType,
                                      VenderName = product.Vendor,
                                      Grades = product.Grades,
                                      GoldWeight = product.GoldWeight,
                                      IsReadyforShip = product.IsReadyforShip,
                                      VenderStyle = product.VenderStyle,
                                      CenterCaratId = size.Id,
                                      CenterShapeId = shape != null ? shape.Id : (int?)null,
                                      CenterCaratName = size.Name,
                                      Quantity = product.Quantity,
                                      KaratId = krt != null ? krt.Id : (int?)null,
                                      Karat = krt.Name,
                                  }).Where(x => x.IsActivated).ToListAsync();

            var groupedProducts = products.GroupBy(p => p.Sku);

            var productDTOList = new List<ProductDTO>();

            foreach (var grp in groupedProducts)
            {
                var firstProduct = grp.First();

                var metals = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.ColorId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.Metal && prod.Sku == firstProduct.Sku
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        SymbolName = col.SymbolName,
                                        IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                    }).Distinct().ToListAsync();

                var caratSizes = await (from col in _context.ProductProperty
                                        join prod in _context.Product on col.Id equals prod.CenterCaratId
                                        join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                        where colN.Name == SD.CaratSize && prod.Sku == firstProduct.Sku
                                        select new ProductPropertyDTO
                                        {
                                            Id = col.Id,
                                            Name = col.Name,
                                            IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                        }).Distinct().ToListAsync();

                var shapes = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.CenterShapeId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.Shape && prod.Sku == firstProduct.Sku
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        IconPath = col.IconPath,
                                        IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                    }).Distinct().ToListAsync();

                

                var productDTO = new ProductDTO
                {
                    Id = firstProduct.Id,
                    Title = firstProduct.Title,
                    CaratName = firstProduct.CaratName,
                    CategoryId = firstProduct.CategoryId,
                    CategoryName = firstProduct.CategoryName,
                    ColorId = firstProduct.ColorId,
                    ColorName = firstProduct.ColorName,
                    ClarityId = firstProduct.ClarityId,
                    ClarityName = firstProduct.ClarityName,
                    ShapeName = firstProduct.ShapeName,
                    ShapeId = firstProduct.ShapeId,
                    UnitPrice = firstProduct.UnitPrice,
                    Price = firstProduct.Price,
                    CenterCaratId = firstProduct.CenterCaratId,
                    CenterCaratName = firstProduct.CenterCaratName,
                    CenterShapeId = firstProduct.CenterShapeId,
                    CenterShapeName = firstProduct.CenterShapeName,
                    IsActivated = firstProduct.IsActivated,
                    CaratSizeId = firstProduct.CaratSizeId,
                    Description = firstProduct.Description,
                    Sku = firstProduct.Sku,
                    ProductType = firstProduct.ProductType,
                    StyleId = firstProduct.StyleId,
                    Metals = metals,
                    CaratSizes = caratSizes,
                    Shapes = shapes,
                    Grades = firstProduct.Grades,
                    BandWidth = firstProduct.BandWidth,
                    GoldWeight = firstProduct.GoldWeight,
                    MMSize = firstProduct.MMSize,
                    VenderStyle = firstProduct.VenderStyle,
                    Length = firstProduct.Length,
                    Karat = firstProduct.Karat,
                    KaratId = firstProduct.KaratId,
                    VenderName = firstProduct.VenderName,
                    WholesaleCost = firstProduct.WholesaleCost,
                    ProductImageVideos = new List<ProductImageAndVideoDTO>(),
                    
                };

                //  await _context.ProductPrices.Where(x => x.ProductId == firstProduct.Id.ToString()).ToListAsync();

                // Step 4: Get the product images for the first product
                var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.Id.ToString()).ToListAsync();

                foreach (var image in productImages)
                {
                    var imageUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.ImageLgId)?.FileUrl ?? "-";
                    var videoUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.VideoId)?.FileUrl ?? "-";

                    var imageVideo = new ProductImageAndVideoDTO
                    {
                        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
                        VideoUrl = string.IsNullOrWhiteSpace(videoUrl) ? null : videoUrl,
                        IsDefault = image.IsDefault,
                    };

                    productDTO.ProductImageVideos.Add(imageVideo);
                }

                // Add the productDTO to the result list
                productDTOList.Add(productDTO);
            }

            // Return products where there are product images/videos
            return productDTOList;
        }
    }
}
