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
            return await _context.ProductCollections.ToListAsync();
        }

        public async Task<ProductStyles> GetProductStyleById(int id)
        {
            return await _context.ProductStyles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ProductStyleDTO>> GetProductStyles()
        {
            try
            {
                var result = await (from pst in _context.ProductStyles
                                    select new ProductStyleDTO
                                    {
                                        Id = pst.Id,
                                        VenderId = pst.VenderId,
                                        StyleName = pst.StyleName,
                                        CoverPageImage = pst.CoverPageImage,
                                        StyleImage = pst.StyleImage,
                                        CategoryId = pst.CategoryId,
                                        IsActivated = pst.IsActivated,
                                    }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<List<ProductCollectionDTO>> GetProductCollections()
        {
            var result = new List<ProductCollectionDTO>();
            result = await (from x in _context.ProductCollections
                            select new ProductCollectionDTO
                            {
                                Id = x.Id,
                                CollectionImage = x.CollectionImage,
                                CollectionName = x.CollectionName,
                                Descriptions = x.Descriptions,
                                IsActivated = x.IsActivated
                            }).ToListAsync();
            return result;
        }

        public async Task<List<ProductMasterDTO>> GetProductStyleItemsList(string status)
        {
            var data = await (from sty in _context.ProductStyleItems
                              join proMstr in _context.ProductMaster on sty.ProductId equals proMstr.Id.ToString()
                              join cat in _context.Category on proMstr.CategoryId equals cat.Id
                              join color in _context.ProductProperty on proMstr.ColorId equals color.Id
                              join shape in _context.ProductProperty on proMstr.CenterShapeId equals shape.Id into shapeGroup
                              from shape in shapeGroup.DefaultIfEmpty()
                              where proMstr.ProductStatus == status
                              select new ProductMasterDTO
                              {
                                  Id = proMstr.Id,
                                  ProductKey = proMstr.ProductKey,
                                  CategoryId = cat.Id,
                                  CategoryName = cat.Name,
                                  ShapeId = shape != null ? shape.Id : 0,
                                  ShapeName = shape != null ? shape.Name : null,
                                  ColorId = color.Id,
                                  ColorName = color.Name,
                                  GroupId = proMstr.GroupId,
                                  IsActive = proMstr.IsActive,
                                  IsSale = proMstr.IsSale,
                                  Title = proMstr.Title,
                                  Price = proMstr.Price,
                                  ProductStatus = proMstr.ProductStatus
                              }).ToListAsync();

            // Load required shape and metal data
            var shapeList = await GetShapeList();
            var colorList = await GetColorList();

            var shapeDT = (from sp in shapeList
                           join dt in data on sp.Id equals dt.ShapeId
                           select new ProductPropertyDTO
                           {
                               Id = sp.Id,
                               Name = sp.Name,
                               Description = sp.Description,
                               DispOrder = sp.DisplayOrder,
                               IconPath = sp.IconPath,
                               IsActive = sp.IsActive.HasValue ? sp.IsActive.Value : false,
                               ParentId = sp.ParentId,
                               SymbolName = sp.SymbolName,
                               Synonyms = sp.Synonyms
                           }).ToList();

            var colorDT = (from sc in colorList
                           join dt in data on sc.Id equals dt.ColorId
                           select new ProductPropertyDTO
                           {
                               Id = sc.Id,
                               Name = sc.Name,
                               Description = sc.Description,
                               DispOrder = sc.DisplayOrder,
                               IconPath = sc.IconPath,
                               IsActive = sc.IsActive.HasValue ? sc.IsActive.Value : false,
                               ParentId = sc.ParentId,
                               SymbolName = sc.SymbolName,
                               Synonyms = sc.Synonyms
                           }).ToList();

            // Load images and videos for all relevant product keys and shapeIds
            var productKeys = data.Select(x => x.ProductKey).Distinct().ToList();
            var shapeIds = data.Select(x => x.ShapeId).Distinct().ToList();
            var colorIds = data.Select(x => x.ColorId).Distinct().ToList();

            var productImages = await _context.ProductImages
                .Where(x => productKeys.Contains(x.ProductId) &&
                            shapeIds.Contains(x.ShapeId ?? 0) &&
                            colorIds.Contains(x.MetalId ?? 0))
                .ToListAsync();

            var fileIds = productImages
                .SelectMany(x => new[] { x.ImageSmId, x.VideoId })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .Distinct()
                .ToList();

            var fileManagerData = await _context.FileManager
                .Where(f => fileIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.FileUrl);
            var productProps = await _context.ProductProperty.ToListAsync();

            // Enrich each item
            foreach (var pm in data)
            {
                pm.Shapes = shapeDT.Where(x => x.Id == pm.ShapeId).Distinct().ToList();
                pm.Metals = colorDT.Where(x => x.Id == pm.ColorId).Distinct().ToList();

                pm.ProductItems = await GetProductDataByProductKey(pm.GroupId);


                pm.CaratSizes = (from col in productProps
                                 join prod in pm.ProductItems on col.Id equals prod.CenterCaratId
                                 join colN in productProps on col.ParentId equals colN.Id into colNGroup
                                 from colN in colNGroup.DefaultIfEmpty()
                                 where colN != null && colN.Name == SD.CaratSize
                                 select new ProductPropertyDTO
                                 {
                                     Id = col.Id,
                                     Name = string.IsNullOrEmpty(col.Name) ? "-" : col.Name,
                                     SymbolName = string.IsNullOrEmpty(col.SymbolName) ? "-" : col.SymbolName,
                                     Description = string.IsNullOrEmpty(col.Description) ? "-" : col.Description,
                                     Synonyms = string.IsNullOrEmpty(col.Synonyms) ? "-" : col.Synonyms,
                                     IsActive = col.IsActive ?? false,
                                     DispOrder = col.DisplayOrder,
                                     IconPath = string.IsNullOrEmpty(col.IconPath) ? "-" : col.IconPath,
                                     ParentId = col.ParentId ?? 0
                                 })
                                 .Distinct()
                                 .OrderByDescending(x => x.Id)
                                 .ToList();

                pm.ProductImageVideos = productImages
                    .Where(x => x.ProductId == pm.ProductKey &&
                                x.MetalId == pm.ColorId &&
                                x.ShapeId == pm.ShapeId)
                    .Select(x => new ProductImageAndVideoDTO
                    {
                        ProductId = x.ProductId,
                        ImageUrl = x.ImageSmId.HasValue && fileManagerData.ContainsKey(x.ImageSmId.Value)
                            ? fileManagerData[x.ImageSmId.Value]
                            : null,
                        VideoUrl = x.VideoId.HasValue && fileManagerData.ContainsKey(x.VideoId.Value)
                            ? fileManagerData[x.VideoId.Value]
                            : null,
                        IsDefault = x.IsDefault
                    }).ToList();
            }

            return data;
        }

        private async Task<int> GetShapeId()
        {
            var shapeDT = await _context.ProductProperty.Where(static x => x.Name == SD.Shape).FirstOrDefaultAsync();
            return shapeDT.Id;
        }

        private async Task<List<ProductProperty>> GetShapeList()
        {
            int shapeId = await GetShapeId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == shapeId && x.IsActive == true).ToListAsync();
            return result;
        }

        private async Task<int> GetColorId()
        {
            var colorDT = await _context.ProductProperty.Where(static x => x.Name == SD.Metal).FirstOrDefaultAsync();
            return colorDT.Id;
        }

        private async Task<List<ProductProperty>> GetColorList()
        {
            int metalId = await GetColorId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == metalId).ToListAsync();
            return result;
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
                        StyleImage = product.StyleImage,
                        CoverPageImage = product.CoverPageImage,
                        CoverPageTitle = product.CoverPageTitle
                    };

                    await _context.ProductStyles.AddAsync(entity);
                }
                else
                {
                    // Update existing entity
                    entity = await _context.ProductStyles.FindAsync(product.Id);

                    if (entity == null)
                        return false;

                    entity.StyleName = !string.IsNullOrEmpty(product.StyleName) ? product.StyleName : entity.StyleName;
                    entity.VenderId = !string.IsNullOrEmpty(product.VenderId) ? product.VenderId : entity.VenderId;
                    entity.UpdatedDate = DateTime.Now;
                    entity.IsActivated = product.IsActivated ?? false;
                    entity.StyleImage = !string.IsNullOrEmpty(product.StyleImage) ? product.StyleImage : entity.StyleImage;
                    entity.CoverPageImage = !string.IsNullOrEmpty(product.CoverPageImage) ? product.CoverPageImage : entity.CoverPageImage;
                    entity.CoverPageTitle = !string.IsNullOrEmpty(product.CoverPageTitle) ? product.CoverPageTitle : entity.CoverPageTitle;
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

        public async Task<bool> AddOrUpdateBulkProductStyle(BulkUpdateStatusRequest data)
        {
            try
            {
                ProductStyleItems entity;
                List<ProductStyleItems> existList = new List<ProductStyleItems>();
                List<ProductStyleItems> addList = new List<ProductStyleItems>();
                foreach (var id in data.Ids)
                {
                    var dt = await _context.ProductStyleItems.Where(x => x.ProductId == id.ToString()).FirstOrDefaultAsync();
                    if (dt != null)
                    {
                        existList.Add(dt);
                    }
                    else
                    {
                        entity = new ProductStyleItems
                        {
                            ProductId = id,
                            StyleId = data.styleId,
                            IsActive = true,
                        };
                        addList.Add(entity);
                    }
                }
                await _context.ProductStyleItems.AddRangeAsync(addList);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<List<ProductDTO>> GetProductDataByProductKey(string groupId)
        {
            var groupedProducts = await (
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
                where product.GroupId == groupId
                select new ProductDTO
                {
                    Id = product.Id,
                    Title = product.Title,
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
                    ProductDate = product.UpdatedDate,
                    Diameter = product.Diameter,
                    CTW = product.CTW,
                    Certificate = product.Certificate,
                    WholesaleCost = product.WholesaleCost,
                    MMSize = product.MMSize,
                    DiaWT = product.DiaWT,
                    NoOfStones = product.NoOfStones,
                    AccentStoneShapeName = ashape.Name,
                    AccentStoneShapeId = product.AccentStoneShapeId,
                    CreatedBy = product.CreatedBy,
                    UpdatedBy = product.UpdatedBy,
                    CreatedDate = product.CreatedDate,
                    UpdatedDate = product.UpdatedDate,
                    DisplayDate = product.UpdatedDate.Value.ToString("dd/MM/yyyy hh:mm tt"),
                    UpdatedPersonName = usr.FirstName
                })
            .OrderByDescending(x => x.Sku)
            .ToListAsync();

            return groupedProducts;
        }

    }
}
