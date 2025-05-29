using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Graph.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class ProductStyleRepository : IProductStyleRepository
    {
        private readonly ApplicationDBContext _context;
        public ProductStyleRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteProductStyle(int styleId)
        {
            try
            {
                var entity = await _context.ProductStyles.FindAsync(styleId);

                if (entity == null)
                    return false;

                _context.ProductStyles.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

        public async Task<List<ProductStyles>> GetProductStyleByCategoryId(int categoryId)
        {
            return await _context.ProductStyles.Where(x => x.CategoryId == categoryId).ToListAsync();
        }

        public async Task<List<ProductCollections>> GetProductCollectionsByCategoryId(int categoryId)
        {
            return await _context.ProductCollections.Where(x => x.CategoryId == categoryId).ToListAsync();
        }

        public async Task<ProductStyles> GetProductStyleById(int id)
        {
            return await _context.ProductStyles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ProductStyles>> GetProductStyles()
        {
            var result = new List<ProductStyles>();
            result = await _context.ProductStyles.ToListAsync();
            return result;
        }


        //public async Task<IEnumerable<ProductDTO>> GetProductStyleItemsList()
        //{
        //    // 1. Get base product data
        //    var baseProducts = await (
        //        from product in _context.Product
        //        join style in _context.ProductStyleItems on product.Id.ToString() equals style.ProductId
        //        join eventSite in _context.EventSites on product.EventId equals eventSite.Id
        //        join karat in _context.ProductProperty on product.KaratId equals karat.Id
        //        join category in _context.Category on product.CategoryId equals category.Id

        //// Optional joins
        //join color in _context.ProductProperty on product.ColorId equals color.Id into colorJoin
        //        from color in colorJoin.DefaultIfEmpty()

        //        join shape in _context.ProductProperty on product.CenterShapeId equals shape.Id into shapeJoin
        //        from shape in shapeJoin.DefaultIfEmpty()

        //        join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityJoin
        //        from clarity in clarityJoin.DefaultIfEmpty()

        //        join caratSize in _context.ProductProperty on product.CenterCaratId equals caratSize.Id into caratJoin
        //        from caratSize in caratJoin.DefaultIfEmpty()

        //        where product.UploadStatus == SD.Activated
        //        select new ProductDTO
        //        {
        //            Id = product.Id,
        //            Sku = product.Sku,
        //            EventId = product.EventId,
        //            EventName = eventSite.EventName,
        //            Title = eventSite.EventName,
        //            BandWidth = product.BandWidth,
        //            Length = product.Length,
        //            CaratName = product.Carat,
        //            CategoryId = category.Id,
        //            CategoryName = category.Name,
        //            ColorId = color.Id,
        //            ColorName = color.Name,
        //            ClarityId = clarity.Id,
        //            ClarityName = clarity.Name,
        //            ShapeName = shape.Name,
        //            CenterShapeName = shape.Name,
        //            CenterCaratId = caratSize.Id,
        //            CenterCaratName = caratSize.Name,
        //            UnitPrice = product.UnitPrice,
        //            Price = product.Price,
        //            Description = product.Description,
        //            Quantity = product.Quantity,
        //            ProductType = category.ProductType,
        //            VenderName = product.Vendor,
        //            Grades = product.Grades,
        //            GoldWeight = product.GoldWeight,
        //            IsReadyforShip = product.IsReadyforShip,
        //            VenderStyle = product.VenderStyle,
        //            KaratId = karat.Id,
        //            Karat = karat.Name,
        //            UploadStatus = product.UploadStatus,
        //            IsActivated = product.IsActivated,
        //            WholesaleCost = product.WholesaleCost,
        //            ProductDate = product.ProductDate,
        //            MMSize = product.MMSize,
        //            CaratSizeId = product.CaratSizeId
        //        }).ToListAsync();

        //    if (!baseProducts.Any())
        //        return baseProducts;

        //    // 2. Get related images
        //    var productIds = baseProducts.Select(p => p.Id.ToString()).ToList();

        //    var productImages = await _context.ProductImages
        //        .Where(img => productIds.Contains(img.ProductId))
        //        .ToListAsync();

        //    var fileManagerIds = productImages.Select(p => p.ImageLgId).Concat(productImages.Select(p => p.VideoId)).Distinct().ToList();

        //    var fileUrls = await _context.FileManager
        //        .Where(f => fileManagerIds.Contains(f.Id))
        //        .ToDictionaryAsync(f => f.Id, f => f.FileUrl ?? "-");

        //    var productImagesDict = productImages
        //        .GroupBy(img => img.ProductId)
        //        .ToDictionary(
        //            g => g.Key,
        //            g => g.Select(img => new ProductImageAndVideoDTO
        //            {
        //                ImageUrl = fileUrls.TryGetValue((int)img.ImageLgId, out var imgUrl) ? imgUrl : "/images/default-product.png",
        //                VideoUrl = fileUrls.TryGetValue((int)img.VideoId, out var vidUrl) ? vidUrl : "-",
        //                IsDefault = img.IsDefault
        //            }).ToList()
        //        );

        //    // 3. Attach images to products
        //    foreach (var product in baseProducts)
        //    {
        //        product.ProductImageVideos = productImagesDict.TryGetValue(product.Id.ToString(), out var imgs)
        //            ? imgs
        //            : new List<ProductImageAndVideoDTO>
        //            {
        //        new ProductImageAndVideoDTO
        //        {
        //            ImageUrl = "/images/default-product.png", // Fallback
        //            VideoUrl = "-",
        //            IsDefault = true
        //        }
        //            };
        //    }

        //    return baseProducts;
        //}

        public async Task<IEnumerable<ProductDTO>> GetProductStyleItemsList()
        {
            var groupedProducts = await (from product in _context.Product
                                         join style in _context.ProductStyleItems on product.Id.ToString() equals style.ProductId
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
                                         where product.UploadStatus == SD.Activated && style.IsActive==true
                                         select new ProductDTO
                                         {
                                             Id = product.Id,
                                             Title = evt.EventName,
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
                                             UploadStatus = product.UploadStatus,
                                             ProductDate = product.UpdatedDate

                                         }).OrderByDescending(X => X.Sku).ToListAsync();

            var productDTOList = new List<ProductDTO>();

            foreach (var firstProduct in groupedProducts)
            {
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

                var prices = await (from pr in _context.ProductPrices
                                    join prod in _context.Product on pr.ProductId equals prod.Id.ToString()
                                    join kt in _context.ProductProperty on pr.ProductId equals kt.ParentId.ToString()
                                    where pr.ProductId == firstProduct.Id.ToString()
                                    select new ProductPriceDTO
                                    {
                                        Id = pr.Id,
                                        KaratName = kt.Name,
                                        ProductId = prod.Id.ToString(),
                                        ProductPrice = prod.Price,
                                        KaratId = pr.KaratId
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
                    Prices = prices,
                    UploadStatus = firstProduct.UploadStatus,
                    IsReadyforShip = firstProduct.IsReadyforShip,
                };

                //  await _context.ProductPrices.Where(x => x.ProductId == firstProduct.Id.ToString()).ToListAsync();

                // Step 4: Get the product images for the first product
                var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.Id.ToString() && x.MetalId == firstProduct.ColorId).ToListAsync();

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


        public async Task<bool> SaveProductStyle(ProductStyleDTO product)
        {
            try
            {
                ProductStyles entity;

                if (product.Id == 0)
                {
                    // Create new entity
                    entity = new ProductStyles
                    {
                        StyleName = product.StyleName,
                        VenderId = product.VenderId,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = null,
                        IsActivated = product.IsActivated ?? false,
                        StyleImage = product.StyleImage
                    };

                    await _context.ProductStyles.AddAsync(entity);
                }
                else
                {
                    // Update existing entity
                    entity = await _context.ProductStyles.FindAsync(product.Id);

                    if (entity == null)
                        return false;

                    entity.StyleName = product.StyleName;
                    entity.VenderId = product.VenderId;
                    entity.UpdatedDate = DateTime.Now;
                    entity.IsActivated = product.IsActivated ?? false;
                    entity.StyleImage = product.StyleImage;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

    }
}
