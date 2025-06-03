using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly ApplicationDBContext _context;
        public CollectionRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<ProductCollections>> GetProductCollections() => await _context.ProductCollections.ToListAsync();
        
        public async Task<ProductCollections> GetProductCollectionById(int id) => await _context.ProductCollections.FirstOrDefaultAsync(x=>x.Id==id);

        //public async Task<List<ProductCollectionItems>> GetProductCollectionItems() => await _context.ProductCollections.ToListAsync();
        public async Task<IEnumerable<ProductDTO>> GetProductCollectionItemsList()
        {
            var groupedProducts = await (
     from cols in _context.ProductCollectionItems
     join product in _context.Product
         on cols.ProductId equals product.Id.ToString() // Assuming ProductId in ProductStyleItems is string GUID
     where product.UploadStatus == SD.Activated
     join cat in _context.Category on product.CategoryId equals cat.Id
     join krt in _context.ProductProperty on product.KaratId equals krt.Id
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
         BandWidth = product.BandWidth,
         Length = product.Length,
         CaratName = product.Carat,
         CategoryId = cat.Id,
         CategoryName = cat.Name,
         ProductType = cat.ProductType,
         ColorId = color != null ? color.Id : (int?)null,
         ColorName = color.Name,
         ClarityId = clarity != null ? clarity.Id : (int?)null,
         ClarityName = clarity.Name,
         CenterShapeId = shape != null ? shape.Id : (int?)null,
         CenterShapeName = shape.Name,
         CenterCaratId = size != null ? size.Id : 0,
         CenterCaratName = size.Name,
         UnitPrice = product.UnitPrice,
         Price = product.Price,
         IsActivated = product.IsActivated,
         CaratSizeId = product.CaratSizeId,
         Description = product.Description,
         Sku = product.Sku,
         VenderName = product.Vendor,
         Grades = product.Grades,
         GoldWeight = product.GoldWeight,
         IsReadyforShip = product.IsReadyforShip,
         VenderStyle = product.VenderStyle,
         Quantity = product.Quantity,
         KaratId = krt.Id,
         Karat = krt.Name,
         UploadStatus = product.UploadStatus,
         ProductDate = product.UpdatedDate
     })
     .OrderByDescending(x => x.Sku)
     .ToListAsync();

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
    }
}
