using B2C_ECommerce.IServices;
using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace B2C_ECommerce.Services
{
    public class ProductService : IProductService
    {
        private ApplicationDBContext _context;
        private IProductRepository _productRepository;
        private IProductPropertyRepository _productPropertyRepository;
        public ProductService(ApplicationDBContext context,
            IProductRepository productRepository, IProductPropertyRepository productPropertyRepository)
        {
            _context = context;
            _productRepository = productRepository;
            _productPropertyRepository = productPropertyRepository;
        }

        public async Task<IEnumerable<ProductMasterDTO>> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10)
        {
            var products = await GetProductStyleDTList(); // this loads full list into memory

            var query = products.AsQueryable();

            var shapeIds = filters.Shapes?.Select(Int32.Parse).ToList();
            var metalIds = filters.Metals?.Select(Int32.Parse).ToList();

            if (shapeIds?.Any() == true)
            {
                query = query.Where(p => p.Shapes.Any(shape => shapeIds.Contains(p.ShapeId)));
            }

            if (metalIds?.Any() == true)
            {
                query = query.Where(p => p.Metals.Any(metal => metalIds.Contains(p.ColorId.Value)));
            }

            if (filters.FromPrice.HasValue && filters.ToPrice.HasValue)
            {
                query = query.Where(p => p.Price.HasValue &&
                                         p.Price.Value >= filters.FromPrice.Value &&
                                         p.Price.Value <= filters.ToPrice.Value);
            }
            else if (filters.FromPrice.HasValue)
            {
                query = query.Where(p => p.Price.HasValue && p.Price.Value >= filters.FromPrice);
            }
            else if (filters.ToPrice.HasValue)
            {
                query = query.Where(p => p.Price.HasValue && p.Price.Value <= filters.ToPrice);
            }

            if (filters.FromCarat.HasValue)
            {
                query = query.Where(p => Convert.ToDecimal(p.CaratSizes) >= filters.FromCarat.Value);
            }

            if (filters.ToCarat.HasValue)
            {
                query = query.Where(p => Convert.ToDecimal(p.CaratSizes) <= filters.ToCarat.Value);
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
                        query = query.OrderBy(p => p.Title);
                        break;
                    case "desc":
                        query = query.OrderByDescending(p => p.Title);
                        break;
                    case "price":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "pricemax":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                }
            }

            try
            {
                var pagedResult = query
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList(); // 🔁 use sync because it's in-memory
                return pagedResult;
            }
            catch (Exception ex)
            {
                string ext = ex.Message;
                throw;
            }
        }


        public async Task<ProductDTO> GetProductByProductId(string productId)
        {
            var imageVideo = new ProductImageAndVideoDTO();
            var RelatedProducts = new List<ProductDTO>();
            string imageUrl = "-";
            string videoUrl = "-";

            var productQuery = await (
                  from product in _context.Product
                  where product.IsActivated && product.ProductKey == productId
                  join cat in _context.Category on product.CategoryId equals cat.Id
                  join karat in _context.ProductProperty on product.KaratId equals karat.Id
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
                      Title = product.Title,
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
                      IsActivated = product.IsActivated,
                      GoldWeight = product.GoldWeight,
                      CenterCaratName = size.Name,
                      Grades = product.Grades,
                      Certificate = product.Certificate,
                      VenderName = product.Vendor,
                      CTW = product.CTW,
                      Diameter = product.Diameter,
                      CenterCaratId = product.CenterCaratId,
                      MMSize = product.MMSize,
                      NoOfStones = product.NoOfStones,
                      DiaWT = product.DiaWT,
                      KaratId = product.KaratId,
                      Karat = karat.Name
                  }
              ).AsNoTracking().FirstOrDefaultAsync();

            // Step 1: Group products by SKU
            var firstProduct = productQuery; // Get the first product from the group

            // Step 2: Get all related properties for each group
            var metals = await (from col in _context.ProductProperty
                                join prod in _context.Product on col.Id equals prod.ColorId
                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                where colN.Name == SD.Metal && prod.Sku == firstProduct.Sku && prod.IsActivated != false
                                select new ProductPropertyDTO
                                {
                                    Id = col.Id,
                                    Name = col.Name,
                                    SymbolName = col.SymbolName,
                                    Synonyms = col.Synonyms,
                                    IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                }).Distinct().ToListAsync();

            var caratSizes = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.CenterCaratId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.CaratSize && prod.Sku == firstProduct.Sku && prod.IsActivated != false
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                    }).Distinct().ToListAsync();

            var shapes = await (from col in _context.ProductProperty
                                join prod in _context.Product on col.Id equals prod.CenterShapeId
                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                where colN.Name == SD.Shape && prod.Sku == firstProduct.Sku && prod.IsActivated != false
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
                GoldWeight = firstProduct.GoldWeight,
                Grades = firstProduct.Grades,
                Certificate = firstProduct.Certificate,
                VenderName = firstProduct.VenderName,
                CTW = firstProduct.CTW,
                Diameter = firstProduct.Diameter,
                MMSize = firstProduct.MMSize,
                NoOfStones = firstProduct.NoOfStones,
                DiaWT = firstProduct.DiaWT,
                KaratId = firstProduct.KaratId,
                Karat = firstProduct.Karat,
                AccentStoneShapeId = firstProduct.AccentStoneShapeId,
                AccentStoneShapeName = firstProduct.AccentStoneShapeName,
                ProductImageVideos = new List<ProductImageAndVideoDTO>() // Initialize to avoid null reference
            };

            // Step 4: Get the product images for the first product
            var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.Id.ToString()).ToListAsync();

            foreach (var image in productImages)
            {
                if (image.ImageLgId.HasValue)
                {
                    imageUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.ImageLgId.Value)?.FileUrl ?? "-";
                    videoUrl = "-";
                }

                else if (image.VideoId.HasValue)
                {
                    videoUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.VideoId.Value)?.FileUrl ?? "-";
                    imageUrl = "-";
                }

                imageVideo = new ProductImageAndVideoDTO
                {
                    ImageUrl = imageUrl,
                    VideoUrl = videoUrl
                };

                productDTO.ProductImageVideos.Add(imageVideo);
            }

            //productDTO.RelatedProducts = await GetJewelleryByShapeColorId(firstProduct.Sku, firstProduct.ColorId.Value, firstProduct.CenterShapeId.Value);
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
                                        Name = !string.IsNullOrEmpty(prd.Name) ? prd.Name : "-",
                                        Description = !string.IsNullOrEmpty(prd.Description) ? prd.Description : "-",
                                        SymbolName = !string.IsNullOrEmpty(prd.SymbolName) ? prd.SymbolName : "-",
                                        Synonyms = !string.IsNullOrEmpty(prd.Synonyms) ? prd.Synonyms : "-",
                                        IconPath = !string.IsNullOrEmpty(prd.IconPath) ? prd.IconPath : "-",
                                        ParentId = !string.IsNullOrEmpty(met.Id.ToString()) ? prd.ParentId : 0,
                                        DispOrder = !string.IsNullOrEmpty(prd.DisplayOrder.ToString()) ? prd.DisplayOrder : 0,
                                        IsActive = prd.IsActive.HasValue ? prd.IsActive.Value : false,
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
                                        CategoryImage = prd.CategoryImage,
                                        IsActivated = prd.IsActivated,
                                        Title = prd.Title,
                                        DisplayOrder = prd.DisplayOrder,
                                        Prefix = prd.Prefix,
                                        SEO_Title = prd.SEO_Title,
                                        SEO_Url = prd.SEO_Url
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


        public async Task<ProductDTO> GetProductsByColorId(string groupId, int? colorId = 0, int? caratId = 0)
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
                                  join ashapeProp in _context.ProductProperty on product.AccentStoneShapeId equals ashapeProp.Id into ashapeGroup
                                  from ashape in ashapeGroup.DefaultIfEmpty()
                                  where product.IsActivated != false && product.GroupId == groupId
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
                                      Karat = kt != null ? kt.Name : null,
                                      Grades = product.Grades,
                                      Certificate = product.Certificate,
                                      VenderName = product.Vendor,
                                      CTW = product.CTW,
                                      Diameter = product.Diameter,
                                      MMSize = product.MMSize,
                                      NoOfStones = product.NoOfStones,
                                      DiaWT = product.DiaWT,
                                      GoldWeight = product.GoldWeight,
                                      AccentStoneShapeId = product.AccentStoneShapeId,
                                      AccentStoneShapeName = ashape.Name,
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
                                where colN.Name == SD.Metal && prod.Sku == firstProduct.Sku && prod.IsActivated != false
                                select new ProductPropertyDTO
                                {
                                    Id = col.Id,
                                    Name = col.Name,
                                    SymbolName = col.SymbolName,
                                    Synonyms = col.Synonyms,
                                    IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                }).Distinct().ToListAsync();

            var caratSizes = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.CenterCaratId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.CaratSize && prod.Sku == firstProduct.Sku && prod.IsActivated != false
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                    }).Distinct().ToListAsync();

            var shapes = await (from col in _context.ProductProperty
                                join prod in _context.Product on col.Id equals prod.CenterShapeId
                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                where colN.Name == SD.Shape && prod.Sku == firstProduct.Sku && prod.IsActivated != false
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
                Grades = firstProduct.Grades,
                Certificate = firstProduct.Certificate,
                VenderName = firstProduct.VenderName,
                CTW = firstProduct.CTW,
                Diameter = firstProduct.Diameter,
                MMSize = firstProduct.MMSize,
                NoOfStones = firstProduct.NoOfStones,
                DiaWT = firstProduct.DiaWT,
                GoldWeight = firstProduct.GoldWeight,
                AccentStoneShapeId = firstProduct.AccentStoneShapeId,
                AccentStoneShapeName = firstProduct.AccentStoneShapeName,
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

        //private async Task<IEnumerable<ProductMasterDTO>> GetProductStyleList()
        //{
        //    // Step 1: Fetch the ProductMasterDTO list
        //    var productMstList = await (from proMst in _context.ProductMaster
        //                                join cat in _context.Category on proMst.CategoryId equals cat.Id
        //                                join color in _context.ProductProperty on proMst.ColorId equals color.Id
        //                                where proMst.IsActive == true && proMst.ProductStatus == "Activated"
        //                                select new ProductMasterDTO
        //                                {
        //                                    Id = proMst.Id,
        //                                    CategoryId = proMst.CategoryId,
        //                                    CategoryName = cat.Name,
        //                                    ColorId = proMst.ColorId,
        //                                    ColorName = color.Name,
        //                                    GroupId = proMst.GroupId,
        //                                    IsActive = proMst.IsActive,
        //                                    IsSale = proMst.IsSale,
        //                                    ProductKey = proMst.ProductKey,
        //                                    ProductStatus = proMst.ProductStatus,
        //                                    ProductItems = new List<ProductDTO>()
        //                                }).ToListAsync();

        //    // Step 2: Fetch all product details and properties (metals, carat sizes, shapes) in bulk
        //    var productKeys = productMstList.Select(p => p.ProductKey).ToList();

        //    var productDetails = await (from product in _context.Product
        //                                join krt in _context.ProductProperty on product.KaratId equals krt.Id
        //                                join cat in _context.Category on product.CategoryId equals cat.Id
        //                                join color in _context.ProductProperty on product.ColorId equals color.Id into colorGroup
        //                                from color in colorGroup.DefaultIfEmpty()
        //                                join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityGroup
        //                                from clarity in clarityGroup.DefaultIfEmpty()
        //                                join size in _context.ProductProperty on product.CenterCaratId equals size.Id into sizeGroup
        //                                from size in sizeGroup.DefaultIfEmpty()
        //                                where product.IsActivated == true && product.UploadStatus == SD.Active && productKeys.Contains(product.ProductKey)
        //                                select new ProductDTO
        //                                {
        //                                    Id = product.Id,
        //                                    Title = product.Title,
        //                                    Sku = product.Sku,
        //                                    ProductKey = product.ProductKey,
        //                                    CategoryId = product.CategoryId,
        //                                    CategoryName = cat.Name,
        //                                    ColorId = color != null ? color.Id : (int?)null,
        //                                    ColorName = color.Name,
        //                                    ClarityId = clarity != null ? clarity.Id : (int?)null,
        //                                    ClarityName = clarity.Name,
        //                                    UnitPrice = product.UnitPrice,
        //                                    Price = product.Price,
        //                                    IsActivated = product.IsActivated,
        //                                    CaratSizeId = product.CaratSizeId,
        //                                    Description = product.Description,
        //                                    VenderName = product.Vendor,
        //                                    Grades = product.Grades,
        //                                    IsReadyforShip = product.IsReadyforShip,
        //                                    VenderStyle = product.VenderStyle,
        //                                    CenterCaratId = size.Id,
        //                                    CenterCaratName = size.Name,
        //                                    Quantity = product.Quantity,
        //                                    KaratId = krt != null ? krt.Id : (int?)null,
        //                                    Karat = krt.Name,
        //                                    ProductType = cat.ProductType,
        //                                    GroupId = product.GroupId
        //                                }).ToListAsync();

        //    // Step 3: Prepare product properties (metals, carat sizes, shapes) for each SKU
        //    var metals = await (from col in _context.ProductProperty
        //                        join prod in _context.Product on col.Id equals prod.ColorId
        //                        join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                        where colN.Name == SD.Metal && productKeys.Contains(prod.ProductKey) && prod.IsActivated
        //                        select new { prod.Sku, col.Id, col.Name, col.SymbolName, col.Synonyms, col.IsActive })
        //                        .Distinct()
        //                        .ToListAsync();

        //    var caratSizes = await (from col in _context.ProductProperty
        //                            join prod in _context.Product on col.Id equals prod.CenterCaratId
        //                            join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                            where colN.Name == SD.CaratSize && productKeys.Contains(prod.ProductKey) && prod.IsActivated
        //                            select new { prod.Sku, col.Id, col.Name, col.IsActive })
        //                            .Distinct()
        //                            .ToListAsync();

        //    var shapes = await (from col in _context.ProductProperty
        //                        join prod in _context.Product on col.Id equals prod.CenterShapeId
        //                        join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                        where colN.Name == SD.Shape && productKeys.Contains(prod.ProductKey) && prod.IsActivated
        //                        select new { prod.Sku, col.Id, col.Name, col.IconPath, col.Synonyms, col.IsActive })
        //                        .Distinct()
        //                        .ToListAsync();

        //    // Step 4: Fetch product images and videos in bulk
        //    var productImages = await _context.ProductImages
        //                                      .Where(x => productKeys.Contains(x.ProductId))
        //                                      .ToListAsync();

        //    var fileManager = await _context.FileManager.ToListAsync();

        //    // Step 5: Map product details to ProductMasterDTO
        //    foreach (var productMstItm in productMstList)
        //    {
        //        var prodDT = productDetails.Where(p => p.ProductKey == productMstItm.ProductKey).ToList();
        //        productMstItm.ProductItems.AddRange(prodDT);

        //        foreach (var product in productMstItm.ProductItems)
        //        {
        //            // Add metals, carat sizes, and shapes to the product
        //            product.Metals = metals.Where(m => m.Sku == product.Sku)
        //                                   .Select(m => new ProductPropertyDTO
        //                                   {
        //                                       Id = m.Id,
        //                                       Name = m.Name,
        //                                       SymbolName = m.SymbolName,
        //                                       Synonyms = m.Synonyms,
        //                                       IsActive = m.IsActive.Value
        //                                   }).ToList();

        //            product.CaratSizes = caratSizes.Where(cs => cs.Sku == product.Sku)
        //                                           .Select(cs => new ProductPropertyDTO
        //                                           {
        //                                               Id = cs.Id,
        //                                               Name = cs.Name,
        //                                               IsActive = cs.IsActive.Value
        //                                           }).ToList();

        //            product.Shapes = shapes.Where(s => s.Sku == product.Sku)
        //                                   .Select(s => new ProductPropertyDTO
        //                                   {
        //                                       Id = s.Id,
        //                                       Name = s.Name,
        //                                       IconPath = s.IconPath,
        //                                       Synonyms = s.Synonyms,
        //                                       IsActive = s.IsActive.Value
        //                                   }).ToList();

        //            // Add product images and videos
        //            var imageVideoList = productImages.Where(img => img.ProductId == product.ProductKey.ToString())
        //                                              .Select(image => new ProductImageAndVideoDTO
        //                                              {
        //                                                  ImageUrl = fileManager.FirstOrDefault(x => x.Id == image.ImageLgId)?.FileUrl ?? "-",
        //                                                  VideoUrl = fileManager.FirstOrDefault(x => x.Id == image.VideoId)?.FileUrl ?? "-",
        //                                                  IsDefault = image.IsDefault
        //                                              }).ToList();

        //            product.ProductImageVideos = imageVideoList;
        //        }
        //    }

        //    // Step 6: Return the optimized list
        //    return productMstList;
        //}

        public async Task<List<ProductMasterDTO>> GetJewelleryByShapeColorId(string sku, int colorId, int? shapeId = 0)
        {
            var productsQuery = from product in _context.Product
                                where product.IsActivated == true && product.Sku == sku && product.ColorId == colorId
                                join cat in _context.Category on product.CategoryId equals cat.Id
                                join karat in _context.ProductProperty on product.KaratId equals karat.Id
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
                                select new
                                {
                                    product,
                                    cat,
                                    karat,
                                    color,
                                    shape,
                                    ashape,
                                    clarity,
                                    size
                                };

            if (shapeId > 0)
            {
                var productList = await (
                    from p in productsQuery
                    where p.product.CenterShapeId == shapeId
                    join prodMst in _context.ProductMaster on p.product.ProductKey equals prodMst.ProductKey
                    select new ProductMasterDTO
                    {
                        Id = prodMst.Id,
                        ProductKey = prodMst.ProductKey,
                        ProductStatus = prodMst.ProductStatus,
                        GroupId = prodMst.GroupId,
                        IsActive = prodMst.IsActive,
                        IsSale = prodMst.IsSale,
                        ProductItems = new List<ProductDTO>
                        {
                            new ProductDTO
                            {
                                Title = p.product.Title,
                                BandWidth = p.product.BandWidth,
                                Length = p.product.Length,
                                CaratName = p.product.Carat,
                                CategoryId = p.cat.Id,
                                CategoryName = p.cat.Name,
                                ProductType = p.cat.ProductType,
                                ColorId = p.product.ColorId,
                                ColorName = p.color.Name,
                                ClarityId = p.product.ClarityId,
                                ClarityName = p.clarity.Name,
                                CenterShapeId = p.product.CenterShapeId,
                                CenterShapeName = p.shape.Name,
                                AccentStoneShapeId = p.product.AccentStoneShapeId,
                                AccentStoneShapeName = p.ashape.Name,
                                CaratSizeId = p.product.CaratSizeId,
                                Description = p.product.Description,
                                Sku = p.product.Sku,
                                UnitPrice = p.product.UnitPrice,
                                Price = p.product.Price,
                                IsActivated = p.product.IsActivated,
                                GoldWeight = p.product.GoldWeight,
                                CenterCaratName = p.size.Name,
                                Grades = p.product.Grades,
                                Certificate = p.product.Certificate,
                                VenderName = p.product.Vendor,
                                CTW = p.product.CTW,
                                Diameter = p.product.Diameter,
                                CenterCaratId = p.product.CenterCaratId,
                                MMSize = p.product.MMSize,
                                NoOfStones = p.product.NoOfStones,
                                DiaWT = p.product.DiaWT,
                                KaratId = p.product.KaratId,
                                Karat = p.karat.Name
                            }
                        }
                    }
                ).AsNoTracking().ToListAsync();

                return productList;
            }
            else
            {
                var products = await productsQuery
                    .Select(p => new ProductDTO
                    {
                        Title = p.product.Title,
                        BandWidth = p.product.BandWidth,
                        Length = p.product.Length,
                        CaratName = p.product.Carat,
                        CategoryId = p.cat.Id,
                        CategoryName = p.cat.Name,
                        ProductType = p.cat.ProductType,
                        ColorId = p.product.ColorId,
                        ColorName = p.color.Name,
                        ClarityId = p.product.ClarityId,
                        ClarityName = p.clarity.Name,
                        CenterShapeId = p.product.CenterShapeId,
                        CenterShapeName = p.shape.Name,
                        AccentStoneShapeId = p.product.AccentStoneShapeId,
                        AccentStoneShapeName = p.ashape.Name,
                        CaratSizeId = p.product.CaratSizeId,
                        Description = p.product.Description,
                        Sku = p.product.Sku,
                        UnitPrice = p.product.UnitPrice,
                        Price = p.product.Price,
                        IsActivated = p.product.IsActivated,
                        GoldWeight = p.product.GoldWeight,
                        CenterCaratName = p.size.Name,
                        Grades = p.product.Grades,
                        Certificate = p.product.Certificate,
                        VenderName = p.product.Vendor,
                        CTW = p.product.CTW,
                        Diameter = p.product.Diameter,
                        CenterCaratId = p.product.CenterCaratId,
                        MMSize = p.product.MMSize,
                        NoOfStones = p.product.NoOfStones,
                        DiaWT = p.product.DiaWT,
                        KaratId = p.product.KaratId,
                        Karat = p.karat.Name
                    }).AsNoTracking().ToListAsync();

                // Wrap in one ProductMasterDTO
                return new List<ProductMasterDTO>
                        {
                            new ProductMasterDTO
                            {
                                Id = 0,
                                ProductKey = "",
                                ProductStatus = null,
                                GroupId = null,
                                IsActive = true,
                                IsSale = false,
                                ProductItems = products
                            }
                        };
            }
        }


        public async Task<IEnumerable<ProductMasterDTO>> GetSelectedProductByIds(int[] productIds)
        {
            var allProducts = await _productRepository.GetProductStyleList();
            return allProducts.Where(x => productIds.Contains(x.Id)).ToList();
        }
        public async Task<List<ProductStyles>> ProductStyleDataList()
        {
            var response = await _context.ProductStyles.ToListAsync();
            return response;
        }

        public async Task<List<ProductCollections>> ProductCollectionDataList()
        {
            var response = await _context.ProductCollections.ToListAsync();
            return response;
        }


        public async Task<List<ProductStyleDTO>> ProgramStylesList()
        {
            var response = await (from proSt in _context.ProductStyles
                                  join cat in _context.Category on proSt.CategoryId equals cat.Id
                                  select new ProductStyleDTO
                                  {
                                      CategoryId = cat.Id,
                                      CategoryName = cat.Name,
                                      IsActivated = proSt.IsActivated,
                                      StyleName = proSt.StyleName,
                                      StyleImage = proSt.StyleImage,
                                      Id = proSt.Id,
                                      CoverPageImage = proSt.CoverPageImage,
                                      CoverPageTitle = proSt.CoverPageTitle,
                                      CreatedDate = proSt.CreatedDate,
                                      UpdatedDate = proSt.UpdatedDate
                                  }).ToListAsync();

            return response;
        }


        public async Task<List<ProductCollectionDTO>> ProductCollectionList()
        {
            var response = await (from proSt in _context.ProductCollections
                                  select new ProductCollectionDTO
                                  {
                                      IsActivated = proSt.IsActivated,
                                      CollectionImage = proSt.CollectionImage,
                                      Id = proSt.Id,
                                      CollectionName = proSt.CollectionName,
                                      Descriptions = proSt.Descriptions,
                                  }).ToListAsync();

            return response;
        }


        private async Task<IEnumerable<ProductDTO>> GetProductStyleList()
        {
            var products = await (from product in _context.Product
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
                                      ProductKey = product.ProductKey,
                                      GroupId = product.GroupId,
                                      Type = product.Type,
                                      IsSuccess = product.IsSuccess
                                  }).Where(x => x.IsActivated).ToListAsync();

            var groupedProducts = products.GroupBy(p => p.Sku);

            var productDTOList = new List<ProductDTO>();

            foreach (var grp in groupedProducts)
            {
                var firstProduct = grp.First();

                var metals = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.ColorId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.Metal && prod.Sku == firstProduct.Sku && prod.IsActivated != false
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        SymbolName = col.SymbolName,
                                        Synonyms = col.Synonyms,
                                        IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                    }).Distinct().ToListAsync();

                var caratSizes = await (from col in _context.ProductProperty
                                        join prod in _context.Product on col.Id equals prod.CenterCaratId
                                        join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                        where colN.Name == SD.CaratSize && prod.Sku == firstProduct.Sku && prod.IsActivated != false
                                        select new ProductPropertyDTO
                                        {
                                            Id = col.Id,
                                            Name = col.Name,
                                            IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
                                        }).Distinct().ToListAsync();

                var shapes = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.CenterShapeId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.Shape && prod.Sku == firstProduct.Sku && prod.IsActivated != false
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        IconPath = col.IconPath,
                                        Synonyms = col.Synonyms,
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
                    Type = firstProduct.Type
                };

                //  await _context.ProductPrices.Where(x => x.ProductId == firstProduct.Id.ToString()).ToListAsync();

                // Step 4: Get the product images for the first product
                var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.ProductKey.ToString()).ToListAsync();

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

        public async Task<IEnumerable<ProductMasterDTO>> GetProductStyleDTList()
        {
            try
            {
                // Step 1: Load main ProductMaster with related data
                var data = await (
                    from pm in _context.ProductMaster
                    join cat in _context.Category on pm.CategoryId equals cat.Id
                    join color in _context.ProductProperty on pm.ColorId equals color.Id
                    join shape in _context.ProductProperty on pm.CenterShapeId equals shape.Id
                    where pm.ColorId != null &&
                          pm.ProductStatus == SD.Activated &&
                          pm.IsActive == true
                    select new
                    {
                        Product = pm,
                        Category = cat,
                        Color = color,
                        Shape = shape
                    }
                ).ToListAsync();

                // Step 2: Project to master DTOs (distinct by ProductKey)
                var dtProMasterList = data
                            .GroupBy(x => x.Product.Sku)
                            .Select(g => g.OrderBy(x => x.Product.DisplayOrder).First()) // <- Now ordered by DisplayOrder
                            .Select(x => new ProductMasterDTO
                            {
                                Id = x.Product.Id,
                                Title = x.Product.Title,
                                ProductKey = x.Product.ProductKey,
                                ShapeId = x.Product.CenterShapeId,
                                CategoryId = x.Category?.Id ?? 0,
                                CategoryName = x.Category?.Name,
                                ColorId = x.Color?.Id ?? 0,
                                ColorName = x.Color?.Name,
                                ShapeName = x.Shape?.Name,
                                GroupId = x.Product.GroupId,
                                IsActive = x.Product.IsActive,
                                IsSale = x.Product.IsSale,
                                Price = x.Product.Price,
                                ProductStatus = x.Product.ProductStatus,
                                Sku = x.Product.Sku,
                                DisplayOrder = x.Product.DisplayOrder
                            })
                            .ToList();


                var skus = dtProMasterList.Select(x => x.Sku).ToList();
                var groupIds = dtProMasterList.Select(x => x.GroupId).ToList();
                var productKeys = dtProMasterList.Select(x => x.ProductKey).ToList();

                // Step 3: Load all products with extended info
                var allProducts = await (
                    from product in _context.Product
                    join usr in _context.ApplicationUser on product.UpdatedBy equals usr.Id
                    join krt in _context.ProductProperty on product.KaratId equals krt.Id
                    join cat in _context.Category on product.CategoryId equals cat.Id
                    join color in _context.ProductProperty on product.ColorId equals color.Id
                    join shape in _context.ProductProperty on product.CenterShapeId equals shape.Id into shapeGroup
                    from shape in shapeGroup.DefaultIfEmpty()
                    join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityGroup
                    from clarity in clarityGroup.DefaultIfEmpty()
                    join size in _context.ProductProperty on product.CenterCaratId equals size.Id into sizeGroup
                    from size in sizeGroup.DefaultIfEmpty()
                    join ashape in _context.ProductProperty on product.AccentStoneShapeId equals ashape.Id into ashapeGroup
                    from ashape in ashapeGroup.DefaultIfEmpty()
                    where groupIds.Contains(product.GroupId)
                    select new ProductDTO
                    {
                        Id = product.Id,
                        Title = product.Title,
                        BandWidth = product.BandWidth,
                        Length = product.Length,
                        CaratName = product.Carat,
                        CategoryId = cat.Id,
                        CategoryName = cat.Name,
                        ColorId = color.Id,
                        ColorName = color.Name,
                        ClarityId = clarity != null ? clarity.Id : 0,
                        ClarityName = string.IsNullOrEmpty(clarity.Name) ? "-" : clarity.Name,
                        ShapeName = string.IsNullOrEmpty(shape.Name) ? "-" : shape.Name,
                        CenterShapeName = string.IsNullOrEmpty(shape.Name) ? "-" : shape.Name,
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
                        CenterCaratId = size != null ? size.Id : 0,
                        CenterShapeId = shape != null ? shape.Id : 0,
                        CenterCaratName = string.IsNullOrEmpty(size.Name) ? "-" : size.Name,
                        Quantity = product.Quantity,
                        KaratId = krt.Id,
                        Karat = krt.Name,
                        UploadStatus = product.UploadStatus,
                        ProductDate = product.UpdatedDate,
                        Diameter = product.Diameter,
                        CTW = product.CTW,
                        Certificate = product.Certificate,
                        WholesaleCost = product.WholesaleCost,
                        MMSize = product.MMSize,
                        DiaWT = product.DiaWT,
                        NoOfStones = product.NoOfStones,
                        AccentStoneShapeName = string.IsNullOrEmpty(ashape.Name) ? "-" : ashape.Name,
                        AccentStoneShapeId = product.AccentStoneShapeId,
                        CreatedBy = product.CreatedBy,
                        UpdatedBy = product.UpdatedBy,
                        CreatedDate = product.CreatedDate,
                        UpdatedDate = product.UpdatedDate,
                        DisplayDate = product.UpdatedDate.HasValue ? product.UpdatedDate.Value.ToString("dd/MM/yyyy hh:mm tt") : "",
                        UpdatedPersonName = usr.FirstName,
                        ProductKey = product.ProductKey
                    }
                ).ToListAsync();

                var metals = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.ColorId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.Metal
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        SymbolName = col.SymbolName,
                                        Description = col.Description,
                                        Synonyms = col.Synonyms,
                                        IsActive = col.IsActive.Value,
                                        DispOrder = col.DisplayOrder,
                                        IconPath = col.IconPath,
                                        ParentId = col.ParentId
                                    }).Distinct().ToListAsync();

                // Get Carat Sizes
                var caratSizes = await (from col in _context.ProductProperty
                                        join prod in _context.Product on col.Id equals prod.CenterCaratId
                                        join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                        where colN.Name == SD.CaratSize
                                        select new ProductPropertyDTO
                                        {
                                            Id = col.Id,
                                            Name = col.Name,
                                            SymbolName = col.SymbolName,
                                            Description = col.Description,
                                            Synonyms = col.Synonyms,
                                            IsActive = col.IsActive ?? false,
                                            DispOrder = col.DisplayOrder,
                                            IconPath = col.IconPath,
                                            ParentId = col.ParentId
                                        }).Distinct().ToListAsync();


                // Get Shapes
                var shapes = await (from col in _context.ProductProperty
                                    join prod in _context.ProductMaster on col.Id equals prod.CenterShapeId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.Shape && skus.Contains(prod.Sku) && prod.IsActive == true
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        SymbolName = col.SymbolName,
                                        Description = col.Description,
                                        Synonyms = col.Synonyms,
                                        IsActive = col.IsActive ?? false,
                                        DispOrder = col.DisplayOrder,
                                        IconPath = col.IconPath,
                                        ParentId = col.ParentId
                                    }).Distinct().ToListAsync();


                // Step 5: Load all ProductImages and FileManager entries at once
                var allProductImages = await _context.ProductImages
                    .Where(x => productKeys.Contains(x.ProductId))
                    .ToListAsync();

                var allFileIds = allProductImages
                    .SelectMany(p => new[] { p.ImageMdId, p.ImageLgId, p.VideoId })
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .Distinct()
                    .ToList();

                var fileMap = await _context.FileManager
                    .Where(f => allFileIds.Contains(f.Id))
                    .ToDictionaryAsync(f => f.Id, f => f.FileUrl);

                // Step 6: Assign related data to each master
                foreach (var master in dtProMasterList)
                {
                    master.Metals = metals;
                    master.CaratSizes = caratSizes;
                    master.Shapes = shapes;

                    master.ProductItems = allProducts
                        .Where(p => p.ProductKey == master.ProductKey && p.CenterShapeId == master.ShapeId)
                        .ToList();

                    var productImages = allProductImages
                        .Where(p => p.ProductId == master.ProductKey &&
                                    p.ShapeId == master.ShapeId &&
                                    p.MetalId == master.ColorId)
                        .ToList();

                    var imageVideos = productImages.Select(image => new ProductImageAndVideoDTO
                    {
                        Id = image.Id,
                        ProductId = image.ProductId,
                        ImageUrl = image.ImageLgId.HasValue && fileMap.TryGetValue(image.ImageLgId.Value, out var imgUrl)
                            ? imgUrl
                            : null,
                        VideoUrl = image.VideoId.HasValue && fileMap.TryGetValue(image.VideoId.Value, out var vidUrl)
                            ? vidUrl
                            : null,
                        DisplayOrder = image.ImageIndexNumber,
                        IsDefault = image.IsDefault
                    }).OrderBy(x => x.DisplayOrder).ToList();

                    master.ProductImageVideos = imageVideos;
                }

                return dtProMasterList;
            }
            catch (Exception ex)
            {
                // Optionally log exception here
                throw;
            }
        }




        public async Task<IEnumerable<ProductMasterDTO>> GetProductMasterByProperty(string sku, int colorId, int shapeId)
        {
            try
            {
                // Step 1: Load main ProductMaster with related data
                var data = await (
                    from pm in _context.ProductMaster
                    join cat in _context.Category on pm.CategoryId equals cat.Id
                    join color in _context.ProductProperty on pm.ColorId equals color.Id
                    join shape in _context.ProductProperty on pm.CenterShapeId equals shape.Id
                    where pm.ColorId != null &&
                          pm.ProductStatus == SD.Activated &&
                          pm.IsActive == true
                    select new
                    {
                        Product = pm,
                        Category = cat,
                        Color = color,
                        Shape = shape
                    }
                ).ToListAsync();

                // Step 2: Project to master DTOs (distinct by ProductKey)
                var dtProMasterList = data
                            .GroupBy(x => x.Product.Sku)
                            .Select(g => g.Where(x => x.Product.Sku == sku 
                                            && x.Product.ColorId == colorId 
                                            && x.Product.CenterShapeId == shapeId).First()) // <- Now ordered by DisplayOrder
                            .Select(x => new ProductMasterDTO
                            {
                                Id = x.Product.Id,
                                Title = x.Product.Title,
                                ProductKey = x.Product.ProductKey,
                                ShapeId = x.Product.CenterShapeId,
                                CategoryId = x.Category?.Id ?? 0,
                                CategoryName = x.Category?.Name,
                                ColorId = x.Color?.Id ?? 0,
                                ColorName = x.Color?.Name,
                                ShapeName = x.Shape?.Name,
                                GroupId = x.Product.GroupId,
                                IsActive = x.Product.IsActive,
                                IsSale = x.Product.IsSale,
                                Price = x.Product.Price,
                                ProductStatus = x.Product.ProductStatus,
                                Sku = x.Product.Sku,
                                DisplayOrder = x.Product.DisplayOrder
                            })
                            .ToList();


                var skus = dtProMasterList.Select(x => x.Sku).ToList();
                var groupIds = dtProMasterList.Select(x => x.GroupId).ToList();
                var productKeys = dtProMasterList.Select(x => x.ProductKey).ToList();

                // Step 3: Load all products with extended info
                var allProducts = await (
                    from product in _context.Product
                    join usr in _context.ApplicationUser on product.UpdatedBy equals usr.Id
                    join krt in _context.ProductProperty on product.KaratId equals krt.Id
                    join cat in _context.Category on product.CategoryId equals cat.Id
                    join color in _context.ProductProperty on product.ColorId equals color.Id
                    join shape in _context.ProductProperty on product.CenterShapeId equals shape.Id into shapeGroup
                    from shape in shapeGroup.DefaultIfEmpty()
                    join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityGroup
                    from clarity in clarityGroup.DefaultIfEmpty()
                    join size in _context.ProductProperty on product.CenterCaratId equals size.Id into sizeGroup
                    from size in sizeGroup.DefaultIfEmpty()
                    join ashape in _context.ProductProperty on product.AccentStoneShapeId equals ashape.Id into ashapeGroup
                    from ashape in ashapeGroup.DefaultIfEmpty()
                    where groupIds.Contains(product.GroupId)
                    select new ProductDTO
                    {
                        Id = product.Id,
                        Title = product.Title,
                        BandWidth = product.BandWidth,
                        Length = product.Length,
                        CaratName = product.Carat,
                        CategoryId = cat.Id,
                        CategoryName = cat.Name,
                        ColorId = color.Id,
                        ColorName = color.Name,
                        ClarityId = clarity != null ? clarity.Id : 0,
                        ClarityName = string.IsNullOrEmpty(clarity.Name) ? "-" : clarity.Name,
                        ShapeName = string.IsNullOrEmpty(shape.Name) ? "-" : shape.Name,
                        CenterShapeName = string.IsNullOrEmpty(shape.Name) ? "-" : shape.Name,
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
                        CenterCaratId = size != null ? size.Id : 0,
                        CenterShapeId = shape != null ? shape.Id : 0,
                        CenterCaratName = string.IsNullOrEmpty(size.Name) ? "-" : size.Name,
                        Quantity = product.Quantity,
                        KaratId = krt.Id,
                        Karat = krt.Name,
                        UploadStatus = product.UploadStatus,
                        ProductDate = product.UpdatedDate,
                        Diameter = product.Diameter,
                        CTW = product.CTW,
                        Certificate = product.Certificate,
                        WholesaleCost = product.WholesaleCost,
                        MMSize = product.MMSize,
                        DiaWT = product.DiaWT,
                        NoOfStones = product.NoOfStones,
                        AccentStoneShapeName = string.IsNullOrEmpty(ashape.Name) ? "-" : ashape.Name,
                        AccentStoneShapeId = product.AccentStoneShapeId,
                        CreatedBy = product.CreatedBy,
                        UpdatedBy = product.UpdatedBy,
                        CreatedDate = product.CreatedDate,
                        UpdatedDate = product.UpdatedDate,
                        DisplayDate = product.UpdatedDate.HasValue ? product.UpdatedDate.Value.ToString("dd/MM/yyyy hh:mm tt") : "",
                        UpdatedPersonName = usr.FirstName,
                        ProductKey = product.ProductKey
                    }
                ).ToListAsync();

                var metals = await (from col in _context.ProductProperty
                                    join prod in _context.Product on col.Id equals prod.ColorId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.Metal
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        SymbolName = col.SymbolName,
                                        Description = col.Description,
                                        Synonyms = col.Synonyms,
                                        IsActive = col.IsActive.Value,
                                        DispOrder = col.DisplayOrder,
                                        IconPath = col.IconPath,
                                        ParentId = col.ParentId
                                    }).Distinct().ToListAsync();

                // Get Carat Sizes
                var caratSizes = await (from col in _context.ProductProperty
                                        join prod in _context.Product on col.Id equals prod.CenterCaratId
                                        join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                        where colN.Name == SD.CaratSize
                                        select new ProductPropertyDTO
                                        {
                                            Id = col.Id,
                                            Name = col.Name,
                                            SymbolName = col.SymbolName,
                                            Description = col.Description,
                                            Synonyms = col.Synonyms,
                                            IsActive = col.IsActive ?? false,
                                            DispOrder = col.DisplayOrder,
                                            IconPath = col.IconPath,
                                            ParentId = col.ParentId
                                        }).Distinct().ToListAsync();


                // Get Shapes
                var shapes = await (from col in _context.ProductProperty
                                    join prod in _context.ProductMaster on col.Id equals prod.CenterShapeId
                                    join colN in _context.ProductProperty on col.ParentId equals colN.Id
                                    where colN.Name == SD.Shape && skus.Contains(prod.Sku) && prod.IsActive == true
                                    select new ProductPropertyDTO
                                    {
                                        Id = col.Id,
                                        Name = col.Name,
                                        SymbolName = col.SymbolName,
                                        Description = col.Description,
                                        Synonyms = col.Synonyms,
                                        IsActive = col.IsActive ?? false,
                                        DispOrder = col.DisplayOrder,
                                        IconPath = col.IconPath,
                                        ParentId = col.ParentId
                                    }).Distinct().ToListAsync();


                // Step 5: Load all ProductImages and FileManager entries at once
                var allProductImages = await _context.ProductImages
                    .Where(x => productKeys.Contains(x.ProductId))
                    .ToListAsync();

                var allFileIds = allProductImages
                    .SelectMany(p => new[] { p.ImageMdId, p.ImageLgId, p.VideoId })
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .Distinct()
                    .ToList();

                var fileMap = await _context.FileManager
                    .Where(f => allFileIds.Contains(f.Id))
                    .ToDictionaryAsync(f => f.Id, f => f.FileUrl);

                // Step 6: Assign related data to each master
                foreach (var master in dtProMasterList)
                {
                    master.Metals = metals;
                    master.CaratSizes = caratSizes;
                    master.Shapes = shapes;

                    master.ProductItems = allProducts
                        .Where(p => p.ProductKey == master.ProductKey && p.CenterShapeId == master.ShapeId)
                        .ToList();

                    var productImages = allProductImages
                        .Where(p => p.ProductId == master.ProductKey &&
                                    p.ShapeId == master.ShapeId &&
                                    p.MetalId == master.ColorId)
                        .ToList();

                    var imageVideos = productImages.Select(image => new ProductImageAndVideoDTO
                    {
                        Id = image.Id,
                        ProductId = image.ProductId,
                        ImageUrl = image.ImageLgId.HasValue && fileMap.TryGetValue(image.ImageLgId.Value, out var imgUrl)
                            ? imgUrl
                            : null,
                        VideoUrl = image.VideoId.HasValue && fileMap.TryGetValue(image.VideoId.Value, out var vidUrl)
                            ? vidUrl
                            : null,
                        DisplayOrder = image.ImageIndexNumber,
                        IsDefault = image.IsDefault
                    }).OrderBy(x => x.DisplayOrder).ToList();

                    master.ProductImageVideos = imageVideos;
                }

                return dtProMasterList;
            }
            catch (Exception ex)
            {
                // Optionally log exception here
                throw;
            }
        }
    }
}
