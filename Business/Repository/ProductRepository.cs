using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using Models;

namespace Business.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _context;

        public ProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByDesignNo(string designNo, int metalId)
        {
            var dtDesign = await _context.Product.Where(x => x.Sku == designNo && x.ColorId == metalId).FirstOrDefaultAsync();
            return dtDesign;
        }

        public async Task<List<Product>> GetProductDataByDesignNo(string designNo, int metalId)
        {
            var dtDesign = await _context.Product.Where(x => x.Sku == designNo && x.ColorId == metalId).ToListAsync();
            return dtDesign;
        }

        public async Task<bool> SaveProductList(List<ProductDTO> products)
        {
            try
            {
                // Step 0: Initilize veriable
                var newProduct = new Product();
                var existingProduct = new Product();
                int styleId = 0;

                var styleDT = new ProductStyles();
                var colors = await GetColorList();
                var categories = await _context.Category.ToListAsync();
                var subCategories = await _context.SubCategory.ToListAsync();
                var clarities = await GetClarityList();
                var carats = await GetCaratList();
                var caratSizes = await _context.ProductCaratSize.ToListAsync();
                var shapes = await GetShapeList();
                //var goldWeight = await GetGoldPurityList();
                //var goldPurity = await GetGoldPurityList();

                // Step 2: Create dictionaries for fast lookup by Name
                var colorDict = colors.ToDictionary(x => x.Name, x => x.Id);
                var categoryDict = categories.ToDictionary(x => x.Name, x => x.Id);
                var subCategoryDict = subCategories.ToDictionary(x => x.Name, x => x.Id);
                var clarityDict = clarities.ToDictionary(x => x.Name, x => x.Id);
                var caratDict = carats.ToDictionary(x => x.Name, x => x.Id);
                var caratSizeDict = caratSizes.ToDictionary(x => x.Name, x => x.Id);
                var shapeDict = shapes.ToDictionary(x => x.Name, x => x.Id);
                //var weightDict = shapes.ToDictionary(x => x.Name, x => x.Id);
                //var purityDict = shapes.ToDictionary(x => x.Name, x => x.Id);

                // Lists for insert and update
                var productList = new List<Product>();
                var updateList = new List<Product>();

                int colorId, subCategoryId, categoryId, clarityId, caratId, caratSizeId, shapeId, goldWeightId, goldPurityId = 0;
                // Step 3: Process each product
                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.ColorName)
                        || string.IsNullOrEmpty(product.CategoryName)
                        || string.IsNullOrEmpty(product.SubCategoryName)
                        || string.IsNullOrEmpty(product.ClarityName)
                        || string.IsNullOrEmpty(product.CaratName)
                        || string.IsNullOrEmpty(product.ShapeName)
                        || string.IsNullOrEmpty(product.CaratSizeName))
                    {
                        continue;
                    }

                    colorId = colorDict.GetValueOrDefault(product.ColorName);
                    categoryId = categoryDict.GetValueOrDefault(product.CategoryName);
                    subCategoryId = subCategoryDict.GetValueOrDefault(product.SubCategoryName);
                    clarityId = clarityDict.GetValueOrDefault(product.ClarityName);
                    caratId = caratDict.GetValueOrDefault(product.CaratName);
                    shapeId = shapeDict.GetValueOrDefault(product.ShapeName);
                    caratSizeId = caratSizeDict.GetValueOrDefault(product.CaratSizeName);
                    //goldPurityId = purityDict.GetValueOrDefault(product.GoldPurity);

                    #region Create style

                    if (string.IsNullOrEmpty(product.StyleName) != true)
                    {
                        styleDT = _context.ProductStyles.Where(x => x.StyleName == product.StyleName).FirstOrDefault();

                        if (styleDT == null)
                        {
                            styleDT = new ProductStyles()
                            {
                                StyleName = product.StyleName,
                                CreatedDate = DateTime.Now,
                                IsActivated = true
                            };

                            await _context.ProductStyles.AddAsync(styleDT);
                            await _context.SaveChangesAsync();
                        }
                        styleId = styleDT.Id;
                    }
                    #endregion

                    existingProduct = await _context.Product
                        .Where(x => x.CategoryId == categoryId
                                    && x.Sku == product.Sku)
                        .FirstOrDefaultAsync();

                    if (existingProduct != null)
                    {
                        // Update existing product
                        existingProduct.Title = $"{product.Title}";
                        existingProduct.Sku = product.Sku;
                        existingProduct.CTW = product.CTW;
                        //existingProduct.Price = product.Price;
                        //existingProduct.UnitPrice = product.UnitPrice;
                        //existingProduct.Quantity = product.Quantity;
                        existingProduct.StyleId = styleId;
                        //existingProduct.IsActivated = product.IsActivated;
                        updateList.Add(existingProduct);
                    }
                    else
                    {
                        newProduct = new Product
                        {
                            Title = $"{product.Title}",
                            Sku = product.Sku,
                            CTW = product.CTW,
                            Vendor = product.VenderName,
                            CategoryId = categoryId,
                            SubCategoryId = subCategoryId,
                            CaratSizeId = caratSizeId,
                            CaratId = caratId,
                            ClarityId = clarityId,
                            Length = product.Length,
                            ColorId = colorId,
                            Description = product.Description,
                            // IsActivated = product.IsActivated,
                            StyleId = styleId,
                            GoldWeight = product.GoldWeight,
                            GoldPurityId = goldPurityId,
                            //Price = product.Price,
                            //UnitPrice = product.UnitPrice,
                            //Quantity = product.Quantity,
                            ProductType = product.ProductType,
                            ShapeId = shapeId,
                            Id = Guid.NewGuid()
                        };

                        productList.Add(newProduct);
                    }

                }

                // Step 4: Bulk insert new products and update existing products
                if (productList.Count > 0)
                {
                    await _context.Product.AddRangeAsync(productList);
                }

                if (updateList.Count > 0)
                {
                    _context.Product.UpdateRange(updateList);
                }

                // Step 5: Save changes to the database
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (You can replace this with your actual logging mechanism)
                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                // You can add more handling if needed (e.g., rethrow or return false)
                return false;
            }
        }

        public async Task<bool> SaveProductCollectionList(List<ProductDTO> products)
        {
            try
            {
                // Step 0: Initilize veriable
                var newProduct = new Product();
                var existingProduct = new Product();

                // Step 1: Fetch all related entities in bulk to avoid repeated database calls
                var colors = await GetColorList();
                var categories = await _context.Category.ToListAsync();
                var subCategories = await _context.SubCategory.ToListAsync();
                var clarities = await GetClarityList();
                var carats = await GetCaratList();
                var caratSizes = await _context.ProductCaratSize.ToListAsync();
                var shapes = await GetShapeList();

                // Step 2: Create dictionaries for fast lookup by Name
                var colorDict = colors.ToDictionary(x => x.Name, x => x.Id);
                var categoryDict = categories.ToDictionary(x => x.Name, x => x.Id);
                var subCategoryDict = subCategories.ToDictionary(x => x.Name, x => x.Id);
                var clarityDict = clarities.ToDictionary(x => x.Name, x => x.Id);
                var caratDict = carats.ToDictionary(x => x.Name, x => x.Id);
                var caratSizeDict = caratSizes.ToDictionary(x => x.Name, x => x.Id);
                var shapeDict = shapes.ToDictionary(x => x.Name, x => x.Id);

                // Lists for insert and update
                var productList = new List<Product>();
                var updateList = new List<Product>();

                int colorId, subCategoryId, categoryId, clarityId, caratId, caratSizeId, shapeId, collectionId = 0;

                // Step 3: Process each product
                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.ColorName)
                        || string.IsNullOrEmpty(product.CategoryName)
                        || string.IsNullOrEmpty(product.SubCategoryName)
                        || string.IsNullOrEmpty(product.ClarityName)
                        || string.IsNullOrEmpty(product.CaratName)
                        || string.IsNullOrEmpty(product.ShapeName)
                        || string.IsNullOrEmpty(product.CaratSizeName))
                    {
                        continue;
                    }

                    colorId = colorDict.GetValueOrDefault(product.ColorName);
                    categoryId = categoryDict.GetValueOrDefault(product.CategoryName);
                    subCategoryId = subCategoryDict.GetValueOrDefault(product.SubCategoryName);
                    clarityId = clarityDict.GetValueOrDefault(product.ClarityName);
                    caratId = caratDict.GetValueOrDefault(product.CaratName);
                    shapeId = shapeDict.GetValueOrDefault(product.ShapeName);
                    caratSizeId = caratSizeDict.GetValueOrDefault(product.CaratSizeName);

                    if (string.IsNullOrEmpty(product.CollectionName) != true)
                    {
                        collectionId = _context.ProductCollections.Where(x => x.CollectionName == product.CollectionName).FirstOrDefault().Id;
                    }

                    // Check if a product already exists based on related field IDs
                    existingProduct = await _context.Product
                        .Where(x => x.ProductType == product.ProductType
                                    && x.CategoryId == categoryId
                                    && x.SubCategoryId == subCategoryId
                                    && x.ClarityId == clarityId
                                    && x.ColorId == colorId
                                    && x.CaratId == caratId
                                    && x.CaratSizeId == caratSizeId
                                    && x.Sku == product.Sku)
                        .FirstOrDefaultAsync();

                    if (existingProduct != null)
                    {
                        // Update existing product
                        existingProduct.Title = $"{product.CategoryName} {product.ColorName} {product.CaratName} {product.ProductType} {product.CollectionName} {product.Sku}";
                        existingProduct.Sku = product.Sku;
                        //existingProduct.Price = product.Price;
                        //existingProduct.UnitPrice = product.UnitPrice;
                        //existingProduct.Quantity = product.Quantity;
                        //existingProduct.IsActivated = product.IsActivated;
                        existingProduct.CollectionsId = collectionId;
                        updateList.Add(existingProduct);
                    }
                    else
                    {
                        // Insert new product
                        newProduct = new Product
                        {
                            Title = $"{product.CategoryName} {product.ColorName} {product.CaratName} {product.ProductType} {product.CollectionName} {product.Sku}",
                            Sku = product.Sku,
                            CategoryId = categoryId,
                            SubCategoryId = subCategoryId,
                            CaratSizeId = caratSizeId,
                            CaratId = caratId,
                            ClarityId = clarityId,
                            ColorId = colorId,
                            Description = product.Description,
                            //IsActivated = product.IsActivated,
                            //GoldWeight = product.GoldWeight,
                            //GoldPurity = product.GoldPurity,
                            //Price = product.Price,
                            //UnitPrice = product.UnitPrice,
                            //Quantity = product.Quantity,
                            ProductType = product.ProductType,
                            ShapeId = shapeId,
                            CollectionsId = collectionId,
                            Id = product.Id
                        };

                        productList.Add(newProduct);
                    }
                }

                // Step 4: Bulk insert new products and update existing products
                if (productList.Count > 0)
                {
                    await _context.Product.AddRangeAsync(productList);
                }

                if (updateList.Count > 0)
                {
                    _context.Product.UpdateRange(updateList);
                }

                // Step 5: Save changes to the database
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (You can replace this with your actual logging mechanism)
                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                // You can add more handling if needed (e.g., rethrow or return false)
                return false;
            }
        }

        public async Task<bool> SaveNewProductCollectionList(List<ProductDTO> products)
        {
            try
            {
                // Step 0: Initilize veriable
                var newProduct = new Product();
                var existingProduct = new Product();

                // Step 1: Fetch all related entities in bulk to avoid repeated database calls

                var categories = await _context.Category.ToListAsync();
                var subCategories = await _context.SubCategory.ToListAsync();


                // Step 2: Create dictionaries for fast lookup by Name
                var categoryDict = categories.ToDictionary(x => x.Name, x => x.Id);
                var subCategoryDict = subCategories.ToDictionary(x => x.Name, x => x.Id);

                // Lists for insert and update
                var productList = new List<Product>();
                var updateList = new List<Product>();

                int subCategoryId, categoryId, collectionId, StyleId;

                // Step 3: Process each product
                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.DesignNo)
                        || string.IsNullOrEmpty(product.CategoryName)
                        || string.IsNullOrEmpty(product.SubCategoryName)
                        || string.IsNullOrEmpty(product.StyleName)
                        || string.IsNullOrEmpty(product.Occasion))
                    {
                        continue;
                    }

                    categoryId = categoryDict.GetValueOrDefault(product.CategoryName);
                    subCategoryId = subCategoryDict.GetValueOrDefault(product.SubCategoryName);

                    if (string.IsNullOrEmpty(product.CollectionName) != true)
                    {
                        var collDT = _context.ProductCollections.Where(x => x.CollectionName == product.CollectionName).FirstOrDefault();
                        if (collDT != null)
                        {
                            collectionId = collDT.Id;
                        }
                        else
                        {
                            ProductCollections collection = new ProductCollections();
                            collection.CollectionName = product.CollectionName;
                            await _context.ProductCollections.AddAsync(collection);
                            await _context.SaveChangesAsync();
                            StyleId = collection.Id;
                        }
                    }

                    if (string.IsNullOrEmpty(product.StyleName) != true)
                    {
                        var styleDT = _context.ProductStyles.Where(x => x.StyleName == product.StyleName).FirstOrDefault();
                        if (styleDT != null)
                        {
                            StyleId = styleDT.Id;
                        }
                        else
                        {
                            ProductStyles style = new ProductStyles();
                            style.StyleName = product.StyleName;
                            await _context.ProductStyles.AddAsync(style);
                            await _context.SaveChangesAsync();
                            StyleId = style.Id;
                        }


                    }

                    // Check if a product already exists based on related field IDs
                    existingProduct = await _context.Product
                        .Where(x => x.ProductType == product.ProductType
                                    && x.CategoryId == categoryId
                                    && x.SubCategoryId == subCategoryId
                                    && x.DesignNo == product.DesignNo)
                        .FirstOrDefaultAsync();

                    if (existingProduct != null)
                    {
                        // Update existing product
                        existingProduct.DesignNo = product.DesignNo;
                        existingProduct.Title = product.Title;
                        // existingProduct.ProductDate = product.ProductDate;
                        existingProduct.Designer = product.Designer;
                        existingProduct.CadDesigner = product.CadDesigner;
                        existingProduct.Remarks = product.Remarks;
                        existingProduct.Carat = product.CaratName;
                        existingProduct.Gender = product.Gender;
                        existingProduct.MfgDesign = product.MfgDesign;
                        existingProduct.Package = product.Package;
                        existingProduct.Occasion = product.Occasion;
                        existingProduct.ParentDesign = product.ParentDesign;
                        //  existingProduct.IsActivated = product.IsActivated;
                        existingProduct.CollectionsId = product.CollectionsId;
                        existingProduct.ProductType = product.ProductType;
                        updateList.Add(existingProduct);
                    }
                    else
                    {
                        // Insert new product
                        newProduct = new Product
                        {
                            Id = product.Id,
                            DesignNo = product.DesignNo,
                            Title = product.Title,
                            // ProductDate = product.ProductDate,
                            Designer = product.Designer,
                            CadDesigner = product.CadDesigner,
                            Remarks = product.Remarks,
                            Carat = product.CaratName,
                            Gender = product.Gender,
                            Package = product.Package,
                            Occasion = product.Occasion,
                            //  IsActivated = product.IsActivated,
                            CollectionsId = product.CollectionsId,
                            ParentDesign = product.ParentDesign,
                            ProductType = product.ProductType,
                            MfgDesign = product.MfgDesign,
                            Vendor = product.VenderName,
                        };

                        productList.Add(newProduct);
                    }
                }

                if (productList.Count > 0)
                {
                    await _context.Product.AddRangeAsync(productList);
                }

                if (updateList.Count > 0)
                {
                    _context.Product.UpdateRange(updateList);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductDetailsByExcel(List<ProductDTO> products)
        {
            try
            {
                var updateList = new List<Product>();
                var existingProduct = new Product();

                var karatList = await GetKaratList();

                var karatDict = karatList.ToDictionary(x => x.Name, x => x.Id);

                int karatId;

                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.DesignNo)
                        || string.IsNullOrEmpty(product.ProductType)
                        || string.IsNullOrEmpty(product.ColorName))
                    {
                        continue;
                    }

                    karatId = karatDict.GetValueOrDefault(product.Karat);

                    existingProduct = await _context.Product
                        .Where(x => x.ProductType == product.ProductType
                                    && x.DesignNo == product.DesignNo)
                        .FirstOrDefaultAsync();

                    if (existingProduct != null)
                    {
                        existingProduct.ProductType = product.ProductType;
                        existingProduct.ShapeId = product.ShapeId;
                        existingProduct.Carat = product.Carat;
                        existingProduct.ColorId = product.ColorId;
                        existingProduct.ClarityId = product.ClarityId;
                        // existingProduct.Quantity = product.Quantity;
                        existingProduct.Setting = product.Setting;
                        existingProduct.KaratId = product.KaratId;
                        updateList.Add(existingProduct);
                    }

                }

                if (updateList.Count > 0)
                {
                    _context.Product.UpdateRange(updateList);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Product>> GetProductCollectionNewList()
        {
            var products = await _context.Product.ToListAsync();
            return products;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductStyleList()
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
                                      Karat = krt.Name
                                      //}).Where(x => x.IsActivated).ToListAsync();
                                  }).ToListAsync();

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
                    Prices = prices
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


        public async Task<IEnumerable<ProductDTO>> GetProductCollectionList()
        {
            var colors = await GetColorList();
            var carats = await GetCaratList();
            var shapes = await GetShapeList();
            var clarities = await GetClarityList();

            var products = await (from product in _context.Product
                                  join cat in _context.Category on product.CategoryId equals cat.Id // INNER JOIN (Category remains unchanged)
                                  join subcat in _context.SubCategory on product.SubCategoryId equals subcat.Id

                                  join color in _context.ProductProperty on product.ColorId equals color.Id into colorGroup
                                  from color in colorGroup.DefaultIfEmpty() // LEFT JOIN

                                  join carat in _context.ProductProperty on product.CaratId equals carat.Id into caratGroup
                                  from carat in caratGroup.DefaultIfEmpty() // LEFT JOIN

                                  join shape in _context.ProductProperty on product.ShapeId equals shape.Id into shapeGroup
                                  from shape in shapeGroup.DefaultIfEmpty() // LEFT JOIN

                                  join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityGroup
                                  from clarity in clarityGroup.DefaultIfEmpty() // LEFT JOIN

                                  join size in _context.ProductProperty on product.CaratSizeId equals size.Id into sizeGroup
                                  from size in sizeGroup.DefaultIfEmpty() // LEFT JOIN

                                  join col in _context.ProductCollections on product.CollectionsId equals col.Id into colGroup
                                  from col in colGroup.DefaultIfEmpty() // LEFT JOIN
                                  select new ProductDTO
                                  {
                                      Id = product.Id,
                                      Title = product.Title,
                                      CaratId = carat != null ? carat.Id : (int?)null,
                                      CaratName = carat != null ? carat.Name : null,
                                      CategoryId = cat.Id,
                                      CategoryName = cat.Name,
                                      ColorId = color != null ? color.Id : (int?)null,
                                      ColorName = color != null ? color.Name : null,
                                      SubCategoryId = subcat != null ? subcat.Id : (int?)null,
                                      SubCategoryName = subcat != null ? subcat.Name : null,
                                      ClarityId = clarity != null ? clarity.Id : (int?)null,
                                      ClarityName = clarity != null ? clarity.Name : null,
                                      ShapeName = shape != null ? shape.Name : null,
                                      ShapeId = shape != null ? shape.Id : (int?)null,
                                      UnitPrice = product.UnitPrice,
                                      Price = product.Price,
                                      IsActivated = product.IsActivated,
                                      CaratSizeId = product.CaratSizeId,
                                      CaratSizeName = size != null ? size.Name : null,
                                      Description = product.Description,
                                      Sku = product.Sku,
                                      ProductType = cat.ProductType,
                                      CollectionsId = product.CollectionsId,
                                      CollectionName = col != null ? col.CollectionName : null
                                  }).Where(x => !String.IsNullOrEmpty(x.CollectionName) && x.IsActivated == true).ToListAsync();

            return products;

        }

        public async Task<int> GetColorId()
        {
            var colorDT = await _context.ProductProperty.Where(static x => x.Name == SD.Metal).FirstOrDefaultAsync();
            return colorDT.Id;
        }

        public async Task<List<ProductProperty>> GetColorList()
        {
            int metalId = await GetColorId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == metalId).ToListAsync();
            return result;
        }

        public async Task<int> GetCaratId()
        {
            var caratDT = await _context.ProductProperty.Where(static x => x.Name == "Center-Carat").FirstOrDefaultAsync();
            return caratDT.Id;
        }

        public async Task<List<ProductProperty>> GetCaratList()
        {
            int caratId = await GetCaratId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == caratId).ToListAsync();
            return result;
        }

        public async Task<int> GetKaratId()
        {
            var caratDT = await _context.ProductProperty.Where(static x => x.Name == "Karat").FirstOrDefaultAsync();
            return caratDT.Id;
        }

        public async Task<List<ProductProperty>> GetKaratList()
        {
            int ktId = await GetKaratId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == ktId).ToListAsync();
            return result;
        }

        public async Task<ProductProperty> GetKaratById(int karatId)
        {
            int ktId = await GetKaratId();
            var result = await _context.ProductProperty.Where(x => x.Id == karatId).FirstOrDefaultAsync();
            return result;
        }
        public async Task<int> GetShapeId()
        {
            var shapeDT = await _context.ProductProperty.Where(static x => x.Name == SD.Shape).FirstOrDefaultAsync();
            return shapeDT.Id;
        }

        public async Task<List<ProductProperty>> GetShapeList()
        {
            int shapeId = await GetShapeId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == shapeId && x.IsActive == true).ToListAsync();
            return result;
        }

        public async Task<int> GetClarityId()
        {
            var clarityDT = await _context.ProductProperty.Where(static x => x.Name == "Clarity").FirstOrDefaultAsync();
            return clarityDT.Id;
        }

        public async Task<List<ProductProperty>> GetClarityList()
        {
            int clarityId = await GetClarityId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == clarityId).ToListAsync();
            return result;
        }

        public async Task<int> GetGoldWeightById()
        {
            var GoldWeightDT = await _context.ProductProperty.Where(static x => x.Name == "GoldWeight").FirstOrDefaultAsync();
            return GoldWeightDT.Id;
        }

        public async Task<List<ProductProperty>> GetGoldWeightList()
        {
            int weightId = await GetGoldWeightById();
            var result = await _context.ProductProperty.Where(x => x.ParentId == weightId).ToListAsync();
            return result;
        }

        public async Task<int> GetGoldPurityById()
        {
            var GoldWeightDT = await _context.ProductProperty.Where(static x => x.Name == "GoldPurity").FirstOrDefaultAsync();
            return GoldWeightDT.Id;
        }

        public async Task<List<ProductProperty>> GetGoldPurityList()
        {
            int weightId = await GetGoldPurityById();
            var result = await _context.ProductProperty.Where(x => x.ParentId == weightId).ToListAsync();
            return result;
        }

        public async Task<int> SaveImageVideoPath(string imgVdoPath)
        {
            if (imgVdoPath == null) return 0;
            var existImg = await _context.FileManager.Where(x => x.FileUrl == imgVdoPath).FirstOrDefaultAsync();

            if (existImg == null)
            {
                var imgVdo = new FileManager();
                imgVdo.FileUrl = imgVdoPath;
                await _context.FileManager.AddAsync(imgVdo);
                await _context.SaveChangesAsync();

                return imgVdo.Id;
            }

            return existImg.Id;
        }

        public async Task<bool> SaveImageVideoAsync(ProductImages ImgVdoData)
        {
            try
            {
                var imgDT = new ProductImages();
                imgDT.ProductId = ImgVdoData.ProductId;
                imgDT.ImageLgId = ImgVdoData.ImageLgId ?? null;
                imgDT.VideoId = ImgVdoData.VideoId ?? null;
                imgDT.MetalId = ImgVdoData.MetalId;
                imgDT.Sku = ImgVdoData.Sku;
                imgDT.ShapeId = ImgVdoData.ShapeId;
                imgDT.IsDefault = true;

                await _context.AddAsync(imgDT);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SaveNewProductList(List<ProductDTO> products, string categoryName)
        {
            try
            {
                // Step 0: Initialize variables
                var styleDT = new ProductStyles();
                var colors = await GetColorList();
                var categories = await _context.Category.ToListAsync();
                var subCategories = await _context.SubCategory.ToListAsync();
                //var clarities = await GetClarityList();
                var carats = await GetCaratList();
                var karats = await GetKaratList();
                var shapes = await GetShapeList();

                // Step 1: Create dictionaries for fast lookup
                var colorDict = colors.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var categoryDict = categories.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                //var clarityDict = clarities.ToDictionary(x => x.Name, x => x.Id);
                var caratDict = carats.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var KaratDict = karats.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var shapeDict = shapes.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

                // Step 2: Retrieve existing products from database in bulk
                var categoryId = categoryDict.GetValueOrDefault(categoryName);
                var existingProducts = await _context.Product
                    .Where(x => x.CategoryId == categoryId)
                    .ToListAsync();

                var productList = new List<Product>();
                var productPrices = new List<ProductPrices>();
                var productWeights = new List<ProductWeight>();

                // Step 3: Process each product
                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.ColorName)
                        || string.IsNullOrEmpty(categoryName)
                        || string.IsNullOrEmpty(product.Karat))
                    {
                        continue;
                    }

                    var colorId = colorDict.GetValueOrDefault(product.ColorName);
                    var caratId = GetProductCarat(product.CenterCaratName);
                    var shapeId = shapeDict.GetValueOrDefault(product.CenterShapeName);
                    var AshapeId = shapeDict.GetValueOrDefault(product.AccentStoneShapeName);
                    var karatId = KaratDict.GetValueOrDefault(product.Karat);

                    var events = await GetEventSitesByName(product.Title);

                    var existingProduct = existingProducts
                        .FirstOrDefault(x => x.ColorId == colorId
                                             && x.KaratId == karatId
                                             && x.Sku == product.Sku
                                             && x.CategoryId.Value == categoryId);


                    if (existingProduct != null)
                    {
                        // Update existing product
                        existingProduct.Title = $"{product.EventName}";
                        existingProduct.Sku = product.Sku;
                        //existingProduct.Price = product.Price;
                        //existingProduct.UnitPrice = product.UnitPrice;
                        //existingProduct.Quantity = product.Quantity;
                        //existingProduct.IsActivated = product.IsActivated;
                        existingProduct.KaratId = karatId;
                        existingProduct.CenterCaratId = caratId;
                        existingProduct.BandWidth = product.BandWidth;
                        existingProduct.GoldWeight = product.GoldWeight;
                        existingProduct.Grades = product.Grades;
                        existingProduct.MMSize = product.MMSize;
                        existingProduct.CTW = product.CTW;
                        existingProduct.Certificate = product.Certificate;
                        existingProduct.CenterShapeId = shapeId;
                        existingProduct.AccentStoneShapeId = AshapeId;
                        existingProduct.IsReadyforShip = product.IsReadyforShip;
                        existingProduct.EventId = events.Id;
                        existingProduct.WholesaleCost = product.WholesaleCost;
                        _context.Product.Update(existingProduct);
                    }
                    else
                    {
                        // Insert new product
                        var newProduct = new Product
                        {
                            Title = $"{product.Title}",
                            WholesaleCost = product.WholesaleCost,
                            Sku = product.Sku ?? throw new ArgumentNullException(nameof(product.Sku)),
                            CategoryId = categoryId,
                            KaratId = karatId,
                            CenterCaratId = caratId,
                            Length = product.Length,
                            ColorId = colorId,
                            Description = product.Description,
                            //IsActivated = product.IsActivated,
                            BandWidth = product.BandWidth,
                            //Price = product.Price > 0 ? product.Price : 0m,
                            //UnitPrice = product.UnitPrice,
                            //Quantity = product.Quantity,
                            ProductType = product.ProductType,
                            GoldWeight = product.GoldWeight,
                            Grades = product.Grades,
                            MMSize = product.MMSize,
                            // NoOfStones = product.NoOfStones,
                            DiaWT = product.DiaWT,
                            CenterShapeId = shapeId,
                            Certificate = product.Certificate,
                            AccentStoneShapeId = AshapeId,
                            IsReadyforShip = product.IsReadyforShip,
                            EventId = events.Id,
                            CTW = product.CTW,
                            Vendor = product.VenderName,
                            VenderStyle = product.VenderStyle,
                            Diameter = product.Diameter,
                            Id = Guid.NewGuid(),
                            UploadStatus = SD.Requested,
                            IsActivated = false,
                            IsSuccess = false
                        };

                        productList.Add(newProduct);

                        // Add associated prices and weights for new product
                        productPrices.Add(new ProductPrices
                        {
                            KaratId = karatId,
                            ProductId = newProduct.Id.ToString(),
                            ProductPrice = product.Price
                        });

                        productWeights.Add(new ProductWeight
                        {
                            KaratId = karatId,
                            ProductId = newProduct.Id.ToString(),
                            Weight = !string.IsNullOrEmpty(product.GoldWeight) ? Convert.ToDecimal(product.GoldWeight) : 0
                        });
                    }
                }

                // Step 4: Perform all database operations in bulk
                if (productList.Any())
                {
                    await _context.Product.AddRangeAsync(productList);
                }
                if (productPrices.Any())
                {
                    await _context.ProductPrices.AddRangeAsync(productPrices);
                }
                if (productWeights.Any())
                {
                    await _context.ProductWeights.AddRangeAsync(productWeights);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SaveEarringsList(List<ProductDTO> products, string categoryName)
        {
            try
            {
                // Step 0: Initilize veriable
                var newProduct = new Product();
                var existingProduct = new Product();

                var styleDT = new ProductStyles();
                var colors = await GetColorList();
                var categories = await _context.Category.ToListAsync();
                var subCategories = await _context.SubCategory.ToListAsync();

                var clarities = await GetClarityList();
                var carats = await GetCaratList(); // caratsize list
                var karats = await GetKaratList();
                var shapes = await GetShapeList();


                //var goldWeight = await GetGoldWeightList();
                //var goldPurity = await GetGoldPurityList();

                // Step 2: Create dictionaries for fast lookup by Name
                var colorDict = colors.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var categoryDict = categories.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                //var subCategoryDict = subCategories.ToDictionary(x => x.Name, x => x.Id);
                var KaratDict = karats.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var shapeDict = shapes.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

                // Lists for insert and update
                var productList = new List<Product>();
                var updateList = new List<Product>();

                int colorId, categoryId, caratId, karatId, shapeId = 0;
                // Step 3: Process each product
                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.ColorName)
                        || string.IsNullOrEmpty(categoryName)
                        || string.IsNullOrEmpty(product.Karat))
                    {
                        continue;
                    }

                    colorId = colorDict.GetValueOrDefault(product.ColorName);
                    categoryId = categoryDict.GetValueOrDefault(categoryName);
                    shapeId = shapeDict.GetValueOrDefault(product.AccentStoneShapeName);
                    karatId = KaratDict.GetValueOrDefault(product.Karat);

                    var events = await GetEventSitesByName(product.EventName);


                    existingProduct = await _context.Product
                        .Where(x => x.CategoryId == categoryId
                                    && x.ColorId == colorId
                                    && x.KaratId == karatId
                                    && x.Sku == product.Sku)
                        .FirstOrDefaultAsync();

                    if (existingProduct != null)
                    {

                        // Update existing product
                        existingProduct.Title = product.EventName;
                        existingProduct.Sku = product.Sku;
                        //existingProduct.Price = product.Price;
                        //existingProduct.UnitPrice = product.UnitPrice;
                        //existingProduct.Quantity = product.Quantity;
                        //existingProduct.IsActivated = product.IsActivated;
                        existingProduct.KaratId = karatId;
                        //existingProduct.ShapeId = shapeId;
                        existingProduct.BandWidth = product.BandWidth;
                        existingProduct.GoldWeight = product.GoldWeight;
                        existingProduct.Grades = product.Grades;
                        existingProduct.MMSize = product.MMSize;
                        existingProduct.CTW = product.CTW;
                        existingProduct.Certificate = product.Certificate;
                        existingProduct.AccentStoneShapeId = shapeId;
                        existingProduct.IsReadyforShip = product.IsReadyforShip;
                        existingProduct.EventId = events.Id;
                        existingProduct.WholesaleCost = product.WholesaleCost;
                        existingProduct.Description = product.Description;
                        _context.Product.Update(existingProduct);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        // Insert new product
                        newProduct = new Product
                        {
                            Title = events.EventName,
                            Sku = product.Sku ?? throw new ArgumentNullException(nameof(product.Sku)), // Ensures Sku is not null
                            CategoryId = categoryId,
                            KaratId = karatId,
                            ColorId = colorId,
                            Description = product.Description,
                            //IsActivated = product.IsActivated,
                            //Price = product.Price > 0 ? product.Price : 0m,
                            //UnitPrice = product.UnitPrice,
                            //Quantity = product.Quantity,
                            ProductType = product.ProductType,
                            GoldWeight = product.GoldWeight,
                            Grades = product.Grades,
                            MMSize = product.MMSize,
                            //NoOfStones = product.NoOfStones,
                            DiaWT = product.DiaWT,
                            Certificate = product.Certificate,
                            AccentStoneShapeId = shapeId,
                            IsReadyforShip = product.IsReadyforShip,
                            EventId = events.Id,
                            CTW = product.CTW,
                            Vendor = product.VenderName,
                            VenderStyle = product.VenderStyle,
                            Diameter = product.Diameter,
                            Id = Guid.NewGuid(),
                            IsActivated = false,
                            IsDelete = false,
                            UploadStatus = SD.Requested
                        };

                        //productList.Add(newProduct);
                        await _context.Product.AddAsync(newProduct);
                        await _context.SaveChangesAsync();

                        await _context.ProductPrices.AddAsync(new ProductPrices
                        {
                            KaratId = karatId,
                            ProductId = newProduct.Id.ToString(),
                            ProductPrice = product.Price
                        });
                        await _context.SaveChangesAsync();

                        var data = new ProductWeight();
                        data.KaratId = karatId;
                        data.ProductId = newProduct.Id.ToString();
                        data.Weight = !string.IsNullOrEmpty(product.GoldWeight) ? Convert.ToDecimal(product.GoldWeight) : 0;

                        await _context.ProductWeights.AddAsync(data);
                        await _context.SaveChangesAsync();
                    }

                }


                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (You can replace this with your actual logging mechanism)
                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                // You can add more handling if needed (e.g., rethrow or return false)
                return false;
            }
        }

        public FileSplitDTO ExtractStyleName(string fileName)
        {
            string nameOnly = Path.GetFileNameWithoutExtension(fileName);
            var parts = nameOnly.Split('-');

            if (parts.Length < 3)
            {
                throw new ArgumentException("Filename format is invalid. Expected format: DESIGNNO-COLORCODE");
            }

            var dtImgVideo = new FileSplitDTO
            {
                DesignNo = $"{parts[0]}-{parts[1]}"
            };

            string colorPart = parts[2];

            if (char.IsDigit(colorPart.Last()))
            {
                string letters = new string(colorPart.TakeWhile(c => !char.IsDigit(c)).ToArray());
                string digits = new string(colorPart.SkipWhile(c => !char.IsDigit(c)).ToArray());

                dtImgVideo.ColorName = letters;
                dtImgVideo.Index = int.TryParse(digits, out int indexVal) ? indexVal : 0;
            }
            else
            {
                dtImgVideo.ColorName = colorPart;
                dtImgVideo.Index = 0; // or null if changed to int?
            }

            return dtImgVideo;
        }




        public async Task<ProductDTO> GetProductWithDetails(string productId)
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


        public async Task<ProductDTO> GetProductByColorId(string sku, int? colorId = 0, int? caratId = 0)
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
                                  join sty in _context.ProductProperty on product.StyleId equals sty.Id into styleGroup
                                  from sty in styleGroup.DefaultIfEmpty()
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
                                      StyleId = product.StyleId,
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


        public async Task<int> GetMetalId(string name)
        {
            var colorId = await GetColorId();

            // Use EF.Functions.Like for partial matching (fuzzy search)
            var color = await _context.ProductProperty
                .Where(x => x.ParentId == colorId && EF.Functions.Like(x.Synonyms, "%" + name + "%"))
                .FirstOrDefaultAsync();

            return color?.Id ?? 0; // If no match found, return 0
        }

        public async Task<EventSites> GetEventSitesByName(string name)
        {

            var eventDT = new EventSites();
            eventDT = await _context.EventSites.Where(x => x.EventName == name).FirstOrDefaultAsync();
            if (eventDT == null)
            {
                eventDT = new EventSites();
                eventDT.EventName = name;
                await _context.EventSites.AddAsync(eventDT);
                await _context.SaveChangesAsync();
            }
            return eventDT;

        }

        public async Task<ProductWeight> GetProductWeightData(ProductWeightDTO weightDTO)
        {
            var weightDT = new ProductWeight();
            weightDT = await _context.ProductWeights.Where(x => x.ProductId == weightDT.ProductId).FirstOrDefaultAsync();
            if (weightDT == null)
            {
                weightDT = new ProductWeight();
                weightDT.Weight = weightDTO.Weight;
                weightDT.ProductId = weightDT.ProductId;
                weightDT.KaratId = weightDT.KaratId;
                await _context.ProductWeights.AddAsync(weightDT);
                await _context.SaveChangesAsync();
            }
            return weightDT;
        }


        public async Task<ProductPrices> GetProductPriceData(ProductPriceDTO price)
        {
            var priceDT = new ProductPrices();
            priceDT = await _context.ProductPrices.Where(x => x.ProductId == priceDT.ProductId).FirstOrDefaultAsync();
            if (priceDT == null)
            {
                priceDT = new ProductPrices();
                priceDT.ProductPrice = price.ProductPrice;
                priceDT.ProductId = price.ProductId;
                priceDT.KaratId = price.KaratId;
                await _context.ProductPrices.AddAsync(priceDT);
                await _context.SaveChangesAsync();
            }
            return priceDT;
        }

        private int GetProductCarat(string carat)
        {
            var caratId = GetCaratId().Result;

            // Use EF.Functions.Like for partial matching (fuzzy search)
            if (!string.IsNullOrEmpty(carat))
            {
                var crt = GetCaratList().Result.Where(x => x.Name == carat.ToString()).FirstOrDefault();
                if (crt != null)
                {
                    return crt.Id;
                }
                else
                {
                    var pp = new ProductProperty
                    {
                        ParentId = caratId,
                        Name = carat.ToString(),
                        IsActive = true,
                    };
                    _context.ProductProperty.Add(pp);
                    _context.SaveChanges();
                    return pp.Id;
                }

            }



            return 0;
        }

        public async Task<int> AddProductFileUploadedHistory(ProductFileUploadHistory productFileUpload)
        {
            try
            {
                await _context.ProductFileUploadHistory.AddAsync(productFileUpload);
                await _context.SaveChangesAsync();

                return productFileUpload.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductUploadRequestList()
        {
            var products = await (from product in _context.Product
                                  //join evt in _context.EventSites on product.EventId equals evt.Id
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
                                  where product.UploadStatus == SD.Requested
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
                                      Karat = krt.Name
                                  //}).Where(x => x.UploadStatus == SD.Requested).ToListAsync();
                                  }).ToListAsync();


            // Return products where there are product images/videos
            return products;
        }

    }
}
