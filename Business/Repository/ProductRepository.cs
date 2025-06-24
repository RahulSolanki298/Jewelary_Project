using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tavis.UriTemplates;

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

        public async Task<List<Product>> GetJewelleryDTByDesignNo(string designNo, int metalId, int shapeId)
        {
            var dtDesign = await _context.Product.Where(x => x.Sku == designNo && x.ColorId == metalId && x.CenterShapeId == shapeId).ToListAsync();
            return dtDesign;
        }

        public async Task<bool> SaveProductList(List<ProductDTO> products)
        {
            try
            {
                // Step 0: Initilize veriable
                var productMstr = new ProductMaster();
                string GroupId = string.Empty;
                string ProductKey = string.Empty;

                var newProduct = new Product();
                var existingProduct = new Product();

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

                int colorId, subCategoryId, categoryId, clarityId, caratId, caratSizeId, shapeId, goldPurityId = 0;
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
                            IsActivated = product.IsActivated,
                            GoldWeight = product.GoldWeight,
                            GoldPurityId = goldPurityId,
                            ProductType = product.ProductType,
                            ShapeId = shapeId,
                            GroupId = $"{product.Sku}-{product.ColorName}",
                            WholesaleCost = product.WholesaleCost,
                            BandWidth = product.BandWidth,
                            Carat = product.Carat,
                            //AccentStoneShapeId=accentId,
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

        //public async Task<IEnumerable<ProductDTO>> GetProductStyleList()
        //{
        //    var groupedProducts = await (
        //        from product in _context.Product
        //        join usr in _context.ApplicationUser on product.UpdatedBy equals usr.Id
        //        join krt in _context.ProductProperty on product.KaratId equals krt.Id
        //        join cat in _context.Category on product.CategoryId equals cat.Id
        //        join color in _context.ProductProperty on product.ColorId equals color.Id
        //        join shape in _context.ProductProperty on product.CenterShapeId equals shape.Id into shapeGroup
        //        from shape in shapeGroup.DefaultIfEmpty()
        //        join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityGroup
        //        from clarity in clarityGroup.DefaultIfEmpty()
        //        join size in _context.ProductProperty on product.CenterCaratId equals size.Id into sizeGroup
        //        from size in sizeGroup.DefaultIfEmpty()
        //        join ashape in _context.ProductProperty on product.AccentStoneShapeId equals ashape.Id into ashapeGroup
        //        from ashape in ashapeGroup.DefaultIfEmpty()
        //        where product.UploadStatus == SD.Activated && product.IsActivated == true
        //        select new ProductDTO
        //        {
        //            Id = product.Id,
        //            Title = product.Title,
        //            BandWidth = product.BandWidth,
        //            Length = product.Length,
        //            CaratName = product.Carat,
        //            CategoryId = cat != null ? cat.Id : (int?)null,
        //            CategoryName = cat.Name,
        //            ColorId = color != null ? color.Id : (int?)null,
        //            ColorName = color.Name,
        //            ClarityId = clarity != null ? clarity.Id : (int?)null,
        //            ClarityName = clarity.Name,
        //            ShapeName = shape.Name,
        //            CenterShapeName = shape.Name,
        //            UnitPrice = product.UnitPrice,
        //            Price = product.Price,
        //            IsActivated = product.IsActivated,
        //            CaratSizeId = product.CaratSizeId,
        //            Description = product.Description,
        //            Sku = product.Sku,
        //            ProductType = cat.ProductType,
        //            VenderName = product.Vendor,
        //            Grades = product.Grades,
        //            GoldWeight = product.GoldWeight,
        //            IsReadyforShip = product.IsReadyforShip,
        //            VenderStyle = product.VenderStyle,
        //            CenterCaratId = size.Id,
        //            CenterShapeId = shape != null ? shape.Id : (int?)null,
        //            CenterCaratName = size.Name,
        //            Quantity = product.Quantity,
        //            KaratId = krt != null ? krt.Id : (int?)null,
        //            Karat = krt.Name,
        //            UploadStatus = product.UploadStatus,
        //            ProductDate = product.UpdatedDate,
        //            Diameter = product.Diameter,
        //            CTW = product.CTW,
        //            Certificate = product.Certificate,
        //            WholesaleCost = product.WholesaleCost,
        //            MMSize = product.MMSize,
        //            DiaWT = product.DiaWT,
        //            NoOfStones = product.NoOfStones,
        //            AccentStoneShapeName = ashape.Name,
        //            AccentStoneShapeId = product.AccentStoneShapeId,
        //            CreatedBy = product.CreatedBy,
        //            UpdatedBy = product.UpdatedBy,
        //            CreatedDate = product.CreatedDate,
        //            UpdatedDate = product.UpdatedDate,
        //            DisplayDate = product.UpdatedDate.Value.ToString("dd/MM/yyyy hh:mm tt"),
        //            UpdatedPersonName = usr.FirstName
        //        })
        //    .OrderByDescending(x => x.Sku)
        //    .ToListAsync();

        //    //var groupedProducts = products.GroupBy(p => p.Sku);

        //    var productDTOList = new List<ProductDTO>();

        //    foreach (var firstProduct in groupedProducts)
        //    {
        //        //var firstProduct = grp.First();

        //        var metals = await (from col in _context.ProductProperty
        //                            join prod in _context.Product on col.Id equals prod.ColorId
        //                            join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                            where colN.Name == SD.Metal && prod.Sku == firstProduct.Sku
        //                            select new ProductPropertyDTO
        //                            {
        //                                Id = col.Id,
        //                                Name = col.Name,
        //                                SymbolName = col.SymbolName,
        //                                IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
        //                            }).Distinct().ToListAsync();

        //        var caratSizes = await (from col in _context.ProductProperty
        //                                join prod in _context.Product on col.Id equals prod.CenterCaratId
        //                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                                where colN.Name == SD.CaratSize && prod.Sku == firstProduct.Sku
        //                                select new ProductPropertyDTO
        //                                {
        //                                    Id = col.Id,
        //                                    Name = col.Name,
        //                                    IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
        //                                }).Distinct().ToListAsync();

        //        var shapes = await (from col in _context.ProductProperty
        //                            join prod in _context.Product on col.Id equals prod.CenterShapeId
        //                            join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                            where colN.Name == SD.Shape && prod.Sku == firstProduct.Sku
        //                            select new ProductPropertyDTO
        //                            {
        //                                Id = col.Id,
        //                                Name = col.Name,
        //                                IconPath = col.IconPath,
        //                                IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
        //                            }).Distinct().ToListAsync();



        //        var productDTO = new ProductDTO
        //        {
        //            Id = firstProduct.Id,
        //            Title = firstProduct.Title,
        //            CaratName = firstProduct.CaratName,
        //            CategoryId = firstProduct.CategoryId,
        //            CategoryName = firstProduct.CategoryName,
        //            ColorId = firstProduct.ColorId,
        //            ColorName = firstProduct.ColorName,
        //            ClarityId = firstProduct.ClarityId,
        //            ClarityName = firstProduct.ClarityName,
        //            ShapeName = firstProduct.ShapeName,
        //            ShapeId = firstProduct.ShapeId,
        //            UnitPrice = firstProduct.UnitPrice,
        //            Price = firstProduct.Price,
        //            CenterCaratId = firstProduct.CenterCaratId,
        //            CenterCaratName = firstProduct.CenterCaratName,
        //            CenterShapeId = firstProduct.CenterShapeId,
        //            CenterShapeName = firstProduct.CenterShapeName,
        //            IsActivated = firstProduct.IsActivated,
        //            CaratSizeId = firstProduct.CaratSizeId,
        //            Description = firstProduct.Description,
        //            Sku = firstProduct.Sku,
        //            ProductType = firstProduct.ProductType,
        //            StyleId = firstProduct.StyleId,
        //            Metals = metals,
        //            CaratSizes = caratSizes,
        //            Shapes = shapes,
        //            Grades = firstProduct.Grades,
        //            BandWidth = firstProduct.BandWidth,
        //            GoldWeight = firstProduct.GoldWeight,
        //            MMSize = firstProduct.MMSize,
        //            VenderStyle = firstProduct.VenderStyle,
        //            Length = firstProduct.Length,
        //            Karat = firstProduct.Karat,
        //            KaratId = firstProduct.KaratId,
        //            VenderName = firstProduct.VenderName,
        //            WholesaleCost = firstProduct.WholesaleCost,
        //            ProductImageVideos = new List<ProductImageAndVideoDTO>(),
        //            Diameter = firstProduct.Diameter,
        //            DiaWT = firstProduct.DiaWT,
        //            CTW = firstProduct.CTW,
        //            UploadStatus = firstProduct.UploadStatus,
        //            Certificate = firstProduct.Certificate,
        //            NoOfStones = firstProduct.NoOfStones,
        //            AccentStoneShapeName = firstProduct.AccentStoneShapeName,
        //            AccentStoneShapeId = firstProduct.AccentStoneShapeId,
        //            CreatedBy = firstProduct.CreatedBy,
        //            UpdatedBy = firstProduct.UpdatedBy,
        //            CreatedDate = firstProduct.CreatedDate,
        //            UpdatedDate = firstProduct.UpdatedDate,
        //            DisplayDate = firstProduct.DisplayDate,
        //            UpdatedPersonName = firstProduct.UpdatedPersonName
        //        };

        //        // Step 4: Get the product images for the first product
        //        var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.Id.ToString()).ToListAsync();

        //        foreach (var image in productImages)
        //        {
        //            var imageUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.ImageMdId)?.FileUrl ?? "-";
        //            var videoUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.VideoId)?.FileUrl ?? "-";

        //            var imageVideo = new ProductImageAndVideoDTO
        //            {
        //                ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
        //                VideoUrl = string.IsNullOrWhiteSpace(videoUrl) ? null : videoUrl,
        //                IsDefault = image.IsDefault,
        //            };

        //            productDTO.ProductImageVideos.Add(imageVideo);
        //        }

        //        productDTOList.Add(productDTO);
        //    }

        //    // Return products where there are product images/videos
        //    return productDTOList;
        //}


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
                                      CollectionName = col != null ? col.CollectionName : null,
                                      CreatedBy = product.CreatedBy,
                                      UpdatedBy = product.UpdatedBy,
                                      CreatedDate = product.CreatedDate,
                                      UpdatedDate = product.UpdatedDate,
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
            var result = await _context.ProductProperty.Where(x => x.ParentId == karatId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<ProductProperty> GetProductShapeId(string ShapeCode, int shapeId)
        {
            var result = await _context.ProductProperty.Where(x => x.SymbolName == ShapeCode && x.Id == shapeId).FirstOrDefaultAsync();
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
                imgDT.ImageSmId = ImgVdoData.ImageSmId ?? null;
                imgDT.ImageMdId = ImgVdoData.ImageMdId ?? null;
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

        public async Task<ProductUploadResult> SaveNewProductList(List<ProductDTO> products, string categoryName, string userId, int fileHistoryId)
        {
            var result = new ProductUploadResult(); // Result object to store success/failure data
            try
            {
                // Step 0: Initialize variables

                var productStyleName = new ProductStyleDTO();
                var styleList = new List<ProductStyleItems>();
                var errorList = new List<string>();
                var existingProduct = new Product();
                var colors = await GetColorList();
                var categories = await _context.Category.ToListAsync();
                var karats = await GetKaratList();
                var shapes = await GetShapeList();
                var caratId = 0;
                var colorId = 0;
                var shapeId = 0;
                var AshapeId = 0;
                var karatId = 0;
                var PreGroupId = string.Empty;

                var colorDict = colors.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var categoryDict = categories.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var KaratDict = karats.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var shapeDict = shapes.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var AshapeDict = shapes.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

                var categoryId = categoryDict.GetValueOrDefault(categoryName);

                var existingProducts = await _context.Product
                    .Where(x => x.CategoryId == categoryId)
                    .ToListAsync();

                var SuccessProductItems = new List<Product>();
                var FailProductItems = new List<Product>();

                var productList = new List<Product>();
                var ExistingProductList = new List<Product>();
                var HistoryProductList = new List<ProductHistory>();
                var newProduct = new Product();
                var productStyleList = new List<ProductStyleDTO>();
                var productCollectionList = new List<ProductCollectionItems>();

                foreach (var product in products)
                {
                    if (fileHistoryId == 0)
                    {
                        fileHistoryId = product.FileHistoryId.Value;
                    }

                    if (string.IsNullOrEmpty(product.Sku)
                        || string.IsNullOrEmpty(product.ColorName)
                        || string.IsNullOrEmpty(categoryName)
                        || string.IsNullOrEmpty(product.Karat))
                    {
                        errorList.Add($"{product.Sku} is failed to added. invalid data.");
                        continue;
                    }

                    caratId = GetProductCarat(product.CenterCaratName);
                    colorId = colorDict.GetValueOrDefault(product.ColorName);
                    shapeId = shapeDict.GetValueOrDefault(product.CenterShapeName);
                    AshapeId = AshapeDict.GetValueOrDefault(product.AccentStoneShapeName);
                    karatId = KaratDict.GetValueOrDefault(product.Karat);

                    if (string.IsNullOrWhiteSpace(product.Title) || string.IsNullOrWhiteSpace(product.ColorName) || string.IsNullOrWhiteSpace(product.Sku) || product.Price == null)
                    {
                        newProduct = new Product
                        {
                            Title = $"{product.Title}",
                            WholesaleCost = product.WholesaleCost,
                            Sku = product.Sku ?? throw new ArgumentNullException(nameof(product.Sku)),
                            CategoryId = categoryId,
                            KaratId = karatId,
                            Length = product.Length,
                            ColorId = colorId,
                            Description = product.Description,
                            BandWidth = product.BandWidth,
                            ProductType = product.ProductType,
                            GoldWeight = product.GoldWeight,
                            Grades = product.Grades,
                            Price = product.Price.HasValue ? product.Price.Value : 0,
                            UnitPrice = product.UnitPrice.HasValue ? product.UnitPrice.Value : 0,
                            MMSize = product.MMSize,
                            NoOfStones = Convert.ToInt32(product.NoOfStones),
                            DiaWT = product.DiaWT,
                            Certificate = product.Certificate,
                            IsReadyforShip = product.IsReadyforShip,
                            CTW = product.CTW,
                            Vendor = product.VenderName,
                            VenderStyle = product.VenderStyle,
                            Diameter = product.Diameter,
                            Id = Guid.NewGuid(),
                            UpdatedDate = DateTime.Now,
                            UpdatedBy = userId,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            FileHistoryId = fileHistoryId,
                            UploadStatus = SD.Pending,
                            IsActivated = false,
                            IsSuccess = false,
                            GroupId = $"{product.Sku}_{product.ColorName}",
                            ProductKey = Guid.NewGuid().ToString()
                        };

                        if (caratId > 0)
                        {
                            newProduct.CenterCaratId = caratId;
                        }
                        if (shapeId > 0)
                        {
                            newProduct.CenterShapeId = shapeId;

                        }
                        if (AshapeId > 0)
                        {
                            newProduct.AccentStoneShapeId = AshapeId;
                        }
                        productList.Add(newProduct);
                        continue;
                    }

                    if (PreGroupId != string.Concat(product.Sku, "_", product.CenterShapeName))
                    {
                        PreGroupId = string.Concat(product.Sku, "_", product.CenterShapeName);
                    }

                    var productMst = await _context.ProductMaster.FirstOrDefaultAsync(x => x.GroupId == PreGroupId && x.ColorName == product.ColorName);

                    if (productMst == null)
                    {
                        productMst = new ProductMaster();
                        productMst.FileHistoryId = fileHistoryId;
                        productMst.ColorId = colorId;
                        productMst.ColorName = product.ColorName;
                        productMst.GroupId = PreGroupId;
                        productMst.ProductStatus = SD.Pending;
                        productMst.ProductKey = Guid.NewGuid().ToString();
                        productMst.CreatedBy = userId;
                        productMst.CreatedDate = DateTime.Now;
                        productMst.UpdatedBy = userId;
                        productMst.UpdatedDate = DateTime.Now;
                        productMst.CategoryId = categoryId;
                        productMst.IsActive = false;
                        productMst.Sku = newProduct.Sku;
                        await _context.ProductMaster.AddAsync(productMst);
                        await _context.SaveChangesAsync();


                        var productHMst = new ProductMasterHistory();
                        productHMst.ProductMasterId = productMst.Id;
                        productMst.ColorId = colorId;
                        productHMst.ColorName = product.ColorName;
                        productHMst.GroupId = PreGroupId;
                        productHMst.ProductStatus = SD.Pending;
                        productHMst.ProductKey = productMst.ProductKey;
                        productHMst.CreatedBy = userId;
                        productHMst.CreatedDate = DateTime.Now;
                        productHMst.UpdatedBy = userId;
                        productHMst.UpdatedDate = DateTime.Now;
                        productHMst.CategoryId = categoryId;
                        productHMst.IsActive = false;
                        productHMst.Sku = newProduct.Sku;
                        await _context.ProductMasterHistory.AddAsync(productHMst);
                        await _context.SaveChangesAsync();
                    }

                    existingProduct = existingProducts
                        .FirstOrDefault(x => x.GroupId == productMst.GroupId && x.ProductKey == productMst.ProductKey);

                    if (existingProduct != null)
                    {
                        existingProduct.Title = $"{product.Title}";
                        existingProduct.Sku = product.Sku;
                        existingProduct.KaratId = karatId;
                        existingProduct.BandWidth = product.BandWidth;
                        existingProduct.GoldWeight = product.GoldWeight;
                        existingProduct.Grades = product.Grades;
                        existingProduct.MMSize = product.MMSize;
                        existingProduct.CTW = product.CTW;
                        existingProduct.Certificate = product.Certificate;
                        existingProduct.CenterShapeId = shapeId;
                        existingProduct.IsReadyforShip = product.IsReadyforShip;
                        existingProduct.WholesaleCost = product.WholesaleCost;
                        existingProduct.Price = product.Price.HasValue ? product.Price.Value : 0;
                        existingProduct.UnitPrice = product.UnitPrice.HasValue ? product.UnitPrice.Value : 0;
                        existingProduct.IsSuccess = true;
                        existingProduct.UploadStatus = SD.Pending;
                        existingProduct.UpdatedBy = userId;
                        existingProduct.UpdatedDate = DateTime.Now;
                        existingProduct.FileHistoryId = fileHistoryId;
                        if (caratId > 0)
                        {
                            existingProduct.CenterCaratId = caratId;
                        }
                        if (shapeId > 0)
                        {
                            existingProduct.CenterShapeId = shapeId;

                        }
                        if (AshapeId > 0)
                        {
                            existingProduct.AccentStoneShapeId = AshapeId;
                        }
                        ExistingProductList.Add(existingProduct);
                        result.SuccessCount++;

                    }
                    else
                    {
                        newProduct = new Product
                        {
                            Title = $"{product.Title}",
                            WholesaleCost = product.WholesaleCost,
                            Sku = product.Sku ?? throw new ArgumentNullException(nameof(product.Sku)),
                            CategoryId = categoryId,
                            KaratId = karatId,
                            Length = product.Length,
                            ColorId = colorId,
                            Description = product.Description,
                            BandWidth = product.BandWidth,
                            ProductType = product.ProductType,
                            GoldWeight = product.GoldWeight,
                            Grades = product.Grades,
                            Price = product.Price.HasValue ? product.Price.Value : 0,
                            UnitPrice = product.UnitPrice.HasValue ? product.UnitPrice.Value : 0,
                            MMSize = product.MMSize,
                            NoOfStones = Convert.ToInt32(product.NoOfStones),
                            DiaWT = product.DiaWT,
                            Certificate = product.Certificate,
                            IsReadyforShip = product.IsReadyforShip,
                            CTW = product.CTW,
                            Vendor = product.VenderName,
                            VenderStyle = product.VenderStyle,
                            Diameter = product.Diameter,
                            Id = Guid.NewGuid(),
                            UpdatedDate = DateTime.Now,
                            UpdatedBy = userId,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            FileHistoryId = fileHistoryId,
                            UploadStatus = SD.Pending,
                            IsActivated = false,
                            IsSuccess = true,
                            GroupId = productMst.GroupId,
                            ProductKey = productMst.ProductKey,
                        };
                        if (caratId > 0)
                        {
                            newProduct.CenterCaratId = caratId;
                        }
                        if (shapeId > 0)
                        {
                            newProduct.CenterShapeId = shapeId;
                        }
                        if (AshapeId > 0)
                        {
                            newProduct.AccentStoneShapeId = AshapeId;
                        }

                        productList.Add(newProduct);
                        result.SuccessCount++;
                    }

                    var styleDT = await _context.ProductStyles.Where(x => x.StyleName == product.StyleName).FirstOrDefaultAsync();
                    if (styleDT == null && product.StyleName != null)
                    {
                        styleDT = new ProductStyles()
                        {
                            StyleName = product.StyleName,
                            CategoryId = categoryId,
                            IsActivated = true,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };
                        await _context.ProductStyles.AddAsync(styleDT);
                        await _context.SaveChangesAsync();
                    }

                    if (styleDT != null)
                    {
                        var productStyleItem = await _context.ProductStyleItems.Where(x => x.StyleId == styleDT.Id
                                                                                && x.ProductId == productMst.ProductKey).FirstOrDefaultAsync();

                        if (!string.IsNullOrEmpty(product.StyleName))
                        {
                            productStyleItem = new ProductStyleItems();
                            productStyleItem.StyleId = styleDT.Id;
                            productStyleItem.UserId = userId;
                            productStyleItem.ProductId = newProduct.ProductKey;
                            productStyleItem.IsActive = true;

                            styleList.Add(productStyleItem);
                        }
                    }

                    var collectionDT = await _context.ProductCollections.Where(x => x.CollectionName == product.CollectionName).FirstOrDefaultAsync();
                    if (collectionDT == null && !string.IsNullOrEmpty(product.CollectionName))
                    {
                        collectionDT = new ProductCollections()
                        {
                            CollectionName = product.CollectionName,
                            IsActivated = true,
                            CreatedDate = DateTime.Now,

                        };
                        await _context.ProductCollections.AddAsync(collectionDT);
                        await _context.SaveChangesAsync();
                    }
                    if (collectionDT != null)
                    {
                        var productCollectionItem = await _context.ProductCollectionItems.Where(x => x.CollectionId == collectionDT.Id && x.ProductId == product.ProductKey).FirstOrDefaultAsync();
                        if (productCollectionItem == null && !string.IsNullOrEmpty(newProduct.Id.ToString()))
                        {
                            productCollectionItem = new ProductCollectionItems();
                            productCollectionItem.CollectionId = collectionDT.Id;
                            productCollectionItem.UserId = userId;
                            productCollectionItem.ProductId = newProduct.ProductKey;
                            productCollectionItem.IsActive = true;
                            productCollectionItem.CreatedBy = userId;
                            productCollectionItem.CreatedDate = DateTime.Now;
                            productCollectionItem.UpdatedBy = userId;
                            productCollectionItem.UpdatedDate = DateTime.Now;

                            productCollectionList.Add(productCollectionItem);
                        }

                    }

                }

                if (productList.Any()) await _context.Product.AddRangeAsync(productList);
                if (ExistingProductList.Any()) _context.Product.UpdateRange(ExistingProductList);
                if (styleList.Any()) await _context.ProductStyleItems.AddRangeAsync(styleList);
                if (productCollectionList.Any()) await _context.ProductCollectionItems.AddRangeAsync(productCollectionList);
                await _context.SaveChangesAsync();


                var productDT = await _context.Product.Where(x => x.FileHistoryId == fileHistoryId).ToListAsync();

                foreach (var product in productDT)
                {
                    var historyResult = new ProductHistory
                    {
                        ProductId = product.Id.ToString(),
                        Title = product.Title,
                        WholesaleCost = product.WholesaleCost,
                        Sku = product.Sku ?? throw new ArgumentNullException(nameof(product.Sku)),
                        CategoryId = product.CategoryId,
                        KaratId = product.KaratId,
                        CenterCaratId = product.CenterCaratId,
                        Length = product.Length,
                        ColorId = product.ColorId,
                        Description = product.Description,
                        BandWidth = product.BandWidth,
                        ProductType = product.ProductType,
                        GoldWeight = product.GoldWeight,
                        Grades = product.Grades,
                        Price = product.Price,
                        UnitPrice = product.UnitPrice,
                        MMSize = product.MMSize,
                        NoOfStones = Convert.ToInt32(product.NoOfStones),
                        DiaWT = product.DiaWT,
                        CenterShapeId = product.CenterShapeId,
                        Certificate = product.Certificate,
                        AccentStoneShapeId = product.AccentStoneShapeId,
                        IsReadyforShip = product.IsReadyforShip,
                        CTW = product.CTW,
                        Vendor = product.Vendor,
                        VenderStyle = product.VenderStyle,
                        Diameter = product.Diameter,
                        Id = Guid.NewGuid(),
                        UpdatedDate = DateTime.Now,
                        UpdatedBy = userId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        FileUploadHistoryId = fileHistoryId,
                        UploadStatus = product.UploadStatus,
                        IsActivated = product.IsActivated,
                        IsSuccess = product.IsSuccess,
                        GroupId = product.GroupId,
                        ProductKey = product.ProductKey

                    };
                    HistoryProductList.Add(historyResult);
                }

                if (HistoryProductList.Count > 0)
                {
                    await _context.ProductHistory.AddRangeAsync(HistoryProductList);
                    await _context.SaveChangesAsync();
                }

                var success = await _context.Product.Where(x => x.FileHistoryId == fileHistoryId && x.IsSuccess == true).ToListAsync();
                var fail = await _context.Product.Where(x => x.FileHistoryId == fileHistoryId && x.IsSuccess == false).ToListAsync();
                result.FailureCount = fail.Count();
                result.SuccessCount = success.Count();

                var history = await _context.ProductFileUploadHistory.Where(x => x.Id == fileHistoryId).FirstOrDefaultAsync();

                if (history != null)
                {
                    history.NoOfSuccess = success.Count();
                    history.NoOfFailed = fail.Count();

                    _context.ProductFileUploadHistory.Update(history);
                    await _context.SaveChangesAsync();
                }

                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Errors.Add($"Exception: {ex.Message}");
                return result;
            }
        }

        public async Task<bool> SaveEarringsList(List<ProductDTO> products, string categoryName, string userId, int fileHistoryId)
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


                // Step 2: Create dictionaries for fast lookup by Name
                var colorDict = colors.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var categoryDict = categories.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var KaratDict = karats.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
                var shapeDict = shapes.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

                // Lists for insert and update
                var productList = new List<Product>();
                var updateList = new List<Product>();

                int colorId, categoryId, karatId, shapeId, centerShapeId, centerCaratId = 0;

                // Step 3: Process each product
                var newProducts = new List<Product>();

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
                    centerShapeId = shapeDict.GetValueOrDefault(product.CenterShapeName);
                    centerCaratId = GetProductCarat(product.CenterCaratName);  // If this returns int, else adapt

                    existingProduct = await _context.Product
                        .Where(x => x.CategoryId == categoryId
                                    && x.ColorId == colorId
                                    && x.KaratId == karatId
                                    && x.Sku == product.Sku)
                        .FirstOrDefaultAsync();

                    if (existingProduct != null)
                    {
                        existingProduct.Title = product.Title;
                        existingProduct.Sku = product.Sku;
                        existingProduct.KaratId = karatId;
                        existingProduct.BandWidth = product.BandWidth;
                        existingProduct.GoldWeight = product.GoldWeight;
                        existingProduct.Grades = product.Grades;
                        existingProduct.MMSize = product.MMSize;
                        existingProduct.CTW = product.CTW;
                        existingProduct.Price = product.Price ?? 0;
                        existingProduct.UnitPrice = product.UnitPrice ?? 0;
                        existingProduct.Certificate = product.Certificate;
                        existingProduct.AccentStoneShapeId = shapeId;
                        existingProduct.IsReadyforShip = product.IsReadyforShip;
                        existingProduct.CenterCaratId = centerCaratId;
                        existingProduct.CenterShapeId = centerShapeId;
                        existingProduct.WholesaleCost = product.WholesaleCost;
                        existingProduct.Description = product.Description;
                        existingProduct.UpdatedBy = userId;
                        existingProduct.FileHistoryId = fileHistoryId;
                        existingProduct.UpdatedDate = DateTime.Now;
                        existingProduct.UploadStatus = product.UploadStatus;

                        _context.Product.Update(existingProduct);
                    }
                    else
                    {
                        newProduct = new Product
                        {
                            Title = product.Title,
                            Sku = product.Sku ?? throw new ArgumentNullException(nameof(product.Sku)),
                            CategoryId = categoryId,
                            KaratId = karatId,
                            ColorId = colorId,
                            Description = product.Description,
                            Length = product.Length,
                            BandWidth = product.BandWidth,
                            CenterCaratId = centerCaratId,
                            CenterShapeId = centerShapeId,  // Fixed here
                            ShapeId = shapeId,
                            NoOfStones = product.NoOfStones ?? 0,
                            ProductType = product.ProductType,
                            GoldWeight = product.GoldWeight,
                            Grades = product.Grades,
                            MMSize = product.MMSize,
                            DiaWT = product.DiaWT,
                            Certificate = product.Certificate,
                            AccentStoneShapeId = shapeId,
                            IsReadyforShip = product.IsReadyforShip,
                            CTW = product.CTW,
                            Vendor = product.VenderName,
                            VenderStyle = product.VenderStyle,
                            Diameter = product.Diameter,
                            WholesaleCost = product.WholesaleCost,
                            Price = product.Price ?? 0,
                            UnitPrice = product.UnitPrice ?? 0,
                            Id = Guid.NewGuid(),
                            IsActivated = false,
                            IsDelete = false,
                            UploadStatus = SD.Pending,
                            UpdatedBy = userId,
                            FileHistoryId = fileHistoryId
                        };

                        newProducts.Add(newProduct);
                    }
                }

                if (newProducts.Any())
                {
                    await _context.Product.AddRangeAsync(newProducts);
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Save failed: {ex.Message} - Inner: {ex.InnerException?.Message}");
                    return false;
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

            if (parts.Length < 4)
            {
                return new FileSplitDTO();
                throw new ArgumentException("Filename format is invalid. Expected format: DESIGNNO-COLORCODE");
            }

            var dtImgVideo = new FileSplitDTO
            {
                DesignNo = $"{parts[0]}-{parts[1]}"
            };

            var IsVerify = _context.Product.Where(x => x.Sku == dtImgVideo.DesignNo).FirstOrDefault();
            if (IsVerify == null)
            {
                dtImgVideo.DesignNo = $"{dtImgVideo.DesignNo}-{parts[2]}";
                var dtDesign = _context.Product.Where(x => x.Sku == dtImgVideo.DesignNo).FirstOrDefault();

                if (dtDesign == null)
                {
                    return new FileSplitDTO();
                }
            }

            string shapePart = parts[2];
            dtImgVideo.ShapeCode = shapePart.ToString();

            string colorPart = parts[3];

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

                if (image.ImageMdId.HasValue)
                {
                    imageUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.ImageMdId.Value)?.FileUrl ?? "-";
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
            var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.Id.ToString() && x.MetalId == colorId && x.ShapeId == firstProduct.CenterShapeId).ToListAsync();

            foreach (var image in productImages)
            {
                var imageUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.ImageMdId)?.FileUrl ?? "-";
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
                //.Where(x => x.ParentId == colorId && EF.Functions.Like(x.Synonyms, "%" + name + "%"))
                .Where(x => x.ParentId == colorId && x.SymbolName == name)
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
                                  join usr in _context.ApplicationUser on product.UpdatedBy equals usr.Id.ToString()
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
                                  where product.UploadStatus == SD.Pending
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
                                      UploadStatus = product.UploadStatus,
                                      UpdatedPersonName = usr.FirstName

                                  }).OrderBy(x => x.Sku).ToListAsync();


            // Return products where there are product images/videos
            return products;
        }

        //public async Task<IEnumerable<ProductDTO>> GetProductHoldList()
        //{
        //    var groupedProducts = await (
        //        from product in _context.Product
        //        join usr in _context.ApplicationUser on product.UpdatedBy equals usr.Id
        //        join krt in _context.ProductProperty on product.KaratId equals krt.Id
        //        join cat in _context.Category on product.CategoryId equals cat.Id
        //        join color in _context.ProductProperty on product.ColorId equals color.Id
        //        join shape in _context.ProductProperty on product.CenterShapeId equals shape.Id into shapeGroup
        //        from shape in shapeGroup.DefaultIfEmpty()
        //        join clarity in _context.ProductProperty on product.ClarityId equals clarity.Id into clarityGroup
        //        from clarity in clarityGroup.DefaultIfEmpty()
        //        join size in _context.ProductProperty on product.CenterCaratId equals size.Id into sizeGroup
        //        from size in sizeGroup.DefaultIfEmpty()
        //        join ashape in _context.ProductProperty on product.AccentStoneShapeId equals ashape.Id into ashapeGroup
        //        from ashape in ashapeGroup.DefaultIfEmpty()
        //        where product.UploadStatus == SD.Hold
        //        select new ProductDTO
        //        {
        //            Id = product.Id,
        //            Title = product.Title,
        //            BandWidth = product.BandWidth,
        //            Length = product.Length,
        //            CaratName = product.Carat,
        //            CategoryId = cat != null ? cat.Id : (int?)null,
        //            CategoryName = cat.Name,
        //            ColorId = color != null ? color.Id : (int?)null,
        //            ColorName = color.Name,
        //            ClarityId = clarity != null ? clarity.Id : (int?)null,
        //            ClarityName = clarity.Name,
        //            ShapeName = shape.Name,
        //            CenterShapeName = shape.Name,
        //            UnitPrice = product.UnitPrice,
        //            Price = product.Price,
        //            IsActivated = product.IsActivated,
        //            CaratSizeId = product.CaratSizeId,
        //            Description = product.Description,
        //            Sku = product.Sku,
        //            ProductType = cat.ProductType,
        //            VenderName = product.Vendor,
        //            Grades = product.Grades,
        //            GoldWeight = product.GoldWeight,
        //            IsReadyforShip = product.IsReadyforShip,
        //            VenderStyle = product.VenderStyle,
        //            CenterCaratId = size.Id,
        //            CenterShapeId = shape != null ? shape.Id : (int?)null,
        //            CenterCaratName = size.Name,
        //            Quantity = product.Quantity,
        //            KaratId = krt != null ? krt.Id : (int?)null,
        //            Karat = krt.Name,
        //            UploadStatus = product.UploadStatus,
        //            ProductDate = product.UpdatedDate,
        //            Diameter = product.Diameter,
        //            CTW = product.CTW,
        //            Certificate = product.Certificate,
        //            WholesaleCost = product.WholesaleCost,
        //            MMSize = product.MMSize,
        //            DiaWT = product.DiaWT,
        //            NoOfStones = product.NoOfStones,
        //            AccentStoneShapeName = ashape.Name,
        //            AccentStoneShapeId = product.AccentStoneShapeId,
        //            CreatedBy = product.CreatedBy,
        //            UpdatedBy = product.UpdatedBy,
        //            CreatedDate = product.CreatedDate,
        //            UpdatedDate = product.UpdatedDate,
        //            DisplayDate = product.UpdatedDate.Value.ToString("dd/MM/yyyy hh:mm tt"),
        //            UpdatedPersonName = usr.FirstName
        //        })
        //    .OrderByDescending(x => x.Sku)
        //    .ToListAsync();

        //    var productDTOList = new List<ProductDTO>();

        //    foreach (var firstProduct in groupedProducts)
        //    {
        //        var metals = await (from col in _context.ProductProperty
        //                            join prod in _context.Product on col.Id equals prod.ColorId
        //                            join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                            where colN.Name == SD.Metal && prod.Sku == firstProduct.Sku
        //                            select new ProductPropertyDTO
        //                            {
        //                                Id = col.Id,
        //                                Name = col.Name,
        //                                SymbolName = col.SymbolName,
        //                                IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
        //                            }).Distinct().ToListAsync();

        //        var caratSizes = await (from col in _context.ProductProperty
        //                                join prod in _context.Product on col.Id equals prod.CenterCaratId
        //                                join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                                where colN.Name == SD.CaratSize && prod.Sku == firstProduct.Sku
        //                                select new ProductPropertyDTO
        //                                {
        //                                    Id = col.Id,
        //                                    Name = col.Name,
        //                                    IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
        //                                }).Distinct().ToListAsync();

        //        var shapes = await (from col in _context.ProductProperty
        //                            join prod in _context.Product on col.Id equals prod.CenterShapeId
        //                            join colN in _context.ProductProperty on col.ParentId equals colN.Id
        //                            where colN.Name == SD.Shape && prod.Sku == firstProduct.Sku
        //                            select new ProductPropertyDTO
        //                            {
        //                                Id = col.Id,
        //                                Name = col.Name,
        //                                IconPath = col.IconPath,
        //                                IsActive = col.IsActive.HasValue ? col.IsActive.Value : false
        //                            }).Distinct().ToListAsync();

        //        var productDTO = new ProductDTO
        //        {
        //            Id = firstProduct.Id,
        //            Title = firstProduct.Title,
        //            CaratName = firstProduct.CaratName,
        //            CategoryId = firstProduct.CategoryId,
        //            CategoryName = firstProduct.CategoryName,
        //            ColorId = firstProduct.ColorId,
        //            ColorName = firstProduct.ColorName,
        //            ClarityId = firstProduct.ClarityId,
        //            ClarityName = firstProduct.ClarityName,
        //            ShapeName = firstProduct.ShapeName,
        //            ShapeId = firstProduct.ShapeId,
        //            UnitPrice = firstProduct.UnitPrice,
        //            Price = firstProduct.Price,
        //            CenterCaratId = firstProduct.CenterCaratId,
        //            CenterCaratName = firstProduct.CenterCaratName,
        //            CenterShapeId = firstProduct.CenterShapeId,
        //            CenterShapeName = firstProduct.CenterShapeName,
        //            IsActivated = firstProduct.IsActivated,
        //            CaratSizeId = firstProduct.CaratSizeId,
        //            Description = firstProduct.Description,
        //            Sku = firstProduct.Sku,
        //            ProductType = firstProduct.ProductType,
        //            StyleId = firstProduct.StyleId,
        //            Metals = metals,
        //            CaratSizes = caratSizes,
        //            Shapes = shapes,
        //            Grades = firstProduct.Grades,
        //            BandWidth = firstProduct.BandWidth,
        //            GoldWeight = firstProduct.GoldWeight,
        //            MMSize = firstProduct.MMSize,
        //            VenderStyle = firstProduct.VenderStyle,
        //            Length = firstProduct.Length,
        //            Karat = firstProduct.Karat,
        //            KaratId = firstProduct.KaratId,
        //            VenderName = firstProduct.VenderName,
        //            WholesaleCost = firstProduct.WholesaleCost,
        //            ProductImageVideos = new List<ProductImageAndVideoDTO>(),
        //            Prices = firstProduct.Prices,
        //            UploadStatus = firstProduct.UploadStatus,
        //            IsReadyforShip = firstProduct.IsReadyforShip,
        //            Diameter = firstProduct.Diameter,
        //            CTW = firstProduct.CTW,
        //            Certificate = firstProduct.Certificate,
        //            DiaWT = firstProduct.DiaWT,
        //            NoOfStones = firstProduct.NoOfStones,
        //            AccentStoneShapeName = firstProduct.AccentStoneShapeName,
        //            AccentStoneShapeId = firstProduct.AccentStoneShapeId,
        //            CreatedBy = firstProduct.CreatedBy,
        //            UpdatedBy = firstProduct.UpdatedBy,
        //            CreatedDate = firstProduct.CreatedDate,
        //            UpdatedDate = firstProduct.UpdatedDate,
        //            DisplayDate = firstProduct.DisplayDate,
        //            UpdatedPersonName = firstProduct.UpdatedPersonName
        //        };

        //        //  await _context.ProductPrices.Where(x => x.ProductId == firstProduct.Id.ToString()).ToListAsync();

        //        // Step 4: Get the product images for the first product
        //        var productImages = await _context.ProductImages.Where(x => x.ProductId == firstProduct.Id.ToString() && x.MetalId == firstProduct.ColorId && x.ShapeId == firstProduct.CenterShapeId).ToListAsync();

        //        foreach (var image in productImages)
        //        {
        //            var imageUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.ImageSmId)?.FileUrl ?? "-";
        //            var videoUrl = _context.FileManager.FirstOrDefault(x => x.Id == image.VideoId)?.FileUrl ?? "-";

        //            var imageVideo = new ProductImageAndVideoDTO
        //            {
        //                ProductId = image.ProductId,
        //                ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
        //                VideoUrl = string.IsNullOrWhiteSpace(videoUrl) ? null : videoUrl,
        //                IsDefault = image.IsDefault,
        //            };

        //            productDTO.ProductImageVideos.Add(imageVideo);
        //        }

        //        // Add the productDTO to the result list
        //        productDTOList.Add(productDTO);
        //    }

        //    // Return products where there are product images/videos
        //    return productDTOList;
        //}



        public async Task<IEnumerable<ProductMasterDTO>> GetProductDeActivatedList()
        {
            var pendingProducts = await (
                from product in _context.Product
                where product.UploadStatus == SD.DeActived
                join proMstr in _context.ProductMaster on product.ProductKey equals proMstr.ProductKey
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
                select new
                {
                    proMstr.Id,
                    proMstr.ProductKey,
                    Product = product,
                    User = usr,
                    Karat = krt,
                    Category = cat,
                    Color = color,
                    Shape = shape,
                    Clarity = clarity,
                    CenterCarat = size,
                    CenterShape = shape,
                    AccentShape = ashape
                }).ToListAsync();

            var grouped = pendingProducts
                .GroupBy(x => new { x.Id, x.ProductKey, x.Category })
                .Select(g => new ProductMasterDTO
                {
                    Id = g.Key.Id,
                    ProductKey = g.Key.ProductKey,
                    CategoryId = g.Key.Category.Id,
                    CategoryName = g.Key.Category.Name,
                    ProductItems = g.Select(x => new ProductDTO
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        BandWidth = x.Product.BandWidth,
                        Length = x.Product.Length,
                        CaratName = x.Product.Carat,
                        CategoryId = x.Category?.Id,
                        CategoryName = x.Category?.Name,
                        ColorId = x.Color?.Id,
                        ColorName = x.Color?.Name,
                        ClarityId = x.Clarity?.Id,
                        ClarityName = x.Clarity?.Name,
                        ShapeName = x.Shape?.Name,
                        CenterShapeName = x.Shape?.Name,
                        UnitPrice = x.Product.UnitPrice,
                        Price = x.Product.Price,
                        IsActivated = x.Product.IsActivated,
                        CaratSizeId = x.Product.CaratSizeId,
                        Description = x.Product.Description,
                        Sku = x.Product.Sku,
                        ProductType = x.Category?.ProductType,
                        VenderName = x.Product.Vendor,
                        Grades = x.Product.Grades,
                        GoldWeight = x.Product.GoldWeight,
                        IsReadyforShip = x.Product.IsReadyforShip,
                        VenderStyle = x.Product.VenderStyle,
                        CenterCaratId = x.CenterCarat?.Id ?? 0,
                        CenterCaratName = x.CenterCarat?.Name,
                        CenterShapeId = x.Shape?.Id ?? 0,
                        Quantity = x.Product.Quantity,
                        KaratId = x.Karat?.Id ?? 0,
                        Karat = x.Karat?.Name,
                        UploadStatus = x.Product.UploadStatus,
                        ProductDate = x.Product.UpdatedDate,
                        Diameter = x.Product.Diameter,
                        CTW = x.Product.CTW,
                        Certificate = x.Product.Certificate,
                        WholesaleCost = x.Product.WholesaleCost,
                        MMSize = x.Product.MMSize,
                        DiaWT = x.Product.DiaWT,
                        NoOfStones = x.Product.NoOfStones,
                        AccentStoneShapeName = x.AccentShape?.Name,
                        AccentStoneShapeId = x.Product.AccentStoneShapeId,
                        CreatedBy = x.Product.CreatedBy,
                        UpdatedBy = x.Product.UpdatedBy,
                        CreatedDate = x.Product.CreatedDate,
                        UpdatedDate = x.Product.UpdatedDate,
                        DisplayDate = x.Product.UpdatedDate?.ToString("dd/MM/yyyy hh:mm tt"),
                        UpdatedPersonName = x.User.FirstName,
                        ProductKey = x.Product.ProductKey,
                        GroupId = x.Product.GroupId
                    }).ToList()
                })
                .OrderByDescending(x => x.ProductItems.First().Sku)
                .ToList();

            // Collect all SKUs and Product IDs to batch query dependent data
            var allSkus = grouped.SelectMany(g => g.ProductItems.Select(p => p.Sku)).Distinct().ToList();
            var allProductIds = grouped.Select(g => g.ProductItems.First().ProductKey.ToString()).ToList();
            var allColorIds = grouped.SelectMany(g => g.ProductItems.Select(p => p.ColorId)).Distinct().ToList();
            var allCenterShapeIds = grouped.SelectMany(g => g.ProductItems.Select(p => p.CenterShapeId)).Distinct().ToList();

            // Load ProductImages and FileManager data in one go
            var imageData = await _context.ProductImages
                .Where(img => allProductIds.Contains(img.ProductId))
                .ToListAsync();

            var allFileIds = imageData.Select(i => i.ImageSmId)
                .Concat(imageData.Select(i => i.VideoId))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var fileManagerData = await _context.FileManager
                .Where(f => allFileIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.FileUrl);

            // Load ProductProperties by type
            var metalParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.Metal)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var caratSizeParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.CaratSize)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var shapeParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.Shape)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var allProperties = await _context.ProductProperty.ToListAsync();

            var metals = allProperties
                .Where(p => p.ParentId == metalParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    SymbolName = p.SymbolName,
                    IsActive = p.IsActive ?? false
                }).ToList();

            var caratSizes = allProperties
                .Where(p => p.ParentId == caratSizeParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsActive = p.IsActive ?? false
                }).ToList();

            var shapes = allProperties
                .Where(p => p.ParentId == shapeParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    IconPath = p.IconPath,
                    IsActive = p.IsActive ?? false
                }).ToList();

            // Now map everything back to each item
            foreach (var master in grouped)
            {
                foreach (var item in master.ProductItems)
                {
                    item.Metals = metals.Where(m => m.Id == item.ColorId).ToList();
                    item.CaratSizes = caratSizes.Where(c => c.Id == item.CenterCaratId).ToList();
                    item.Shapes = shapes.Where(s => s.Id == item.CenterShapeId).ToList();

                    item.ProductImageVideos = imageData
                        .Where(img => img.ProductId == item.ProductKey.ToString()
                                   && img.MetalId == item.ColorId
                                   && img.ShapeId == item.CenterShapeId)
                        .Select(img => new ProductImageAndVideoDTO
                        {
                            ProductId = img.ProductId,
                            ImageUrl = fileManagerData.ContainsKey(img.ImageSmId ?? 0) ? fileManagerData[img.ImageSmId.Value] : null,
                            VideoUrl = fileManagerData.ContainsKey(img.VideoId ?? 0) ? fileManagerData[img.VideoId.Value] : null,
                            IsDefault = img.IsDefault
                        }).ToList();
                }
            }

            return grouped;
        }
        public async Task<bool> UpdateProductStatus(string[] productIds, string status, string userId)
        {
            try
            {
                var idList = productIds.ToList();

                // Get all ProductMasters in one query
                var productMasters = await _context.ProductMaster
                                                   .Where(x => idList.Contains(x.Id.ToString()))
                                                   .ToListAsync();

                // Get all corresponding ProductKeys
                var productKeys = productMasters.Select(pm => pm.ProductKey).Distinct().ToList();

                // Get all Products in one query
                var products = await _context.Product
                                             .Where(p => productKeys.Contains(p.ProductKey))
                                             .ToListAsync();

                foreach (var master in productMasters)
                {
                    master.ProductStatus = status;
                    master.IsActive = status == SD.Activated;
                }

                foreach (var product in products)
                {
                    product.UploadStatus = status;
                    product.UpdatedDate = DateTime.Now;
                    product.UpdatedBy = userId;
                    product.IsActivated = status == SD.Activated;
                }

                _context.ProductMaster.UpdateRange(productMasters);
                _context.Product.UpdateRange(products);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Optional: log exception here
                return false;
            }
        }


        public async Task<List<ProductImageAndVideoDTO>> GetProductImagesVideos(string productKey)
        {
            var proImgVideos = new List<ProductImageAndVideoDTO>();
            var proImgVideo = new ProductImageAndVideoDTO();
            var productImg = await _context.ProductImages.Where(x => x.ProductId == productKey).ToListAsync();

            if (productImg.Count > 0)
            {
                foreach (var item in productImg)
                {
                    var img = await _context.FileManager.Where(x => x.Id == item.ImageSmId).FirstOrDefaultAsync();
                    proImgVideo = new ProductImageAndVideoDTO();
                    if (img != null)
                    {
                        proImgVideo.ImageUrl = img.FileUrl;
                        proImgVideo.Id = item.Id;
                        proImgVideo.ProductId = item.ProductId;
                        proImgVideo.IsDefault = item.IsDefault;
                        proImgVideo.VideoUrl = "-";
                        proImgVideos.Add(proImgVideo);

                    }
                    else
                    {
                        var Vdo = await _context.FileManager.Where(x => x.Id == item.VideoId).FirstOrDefaultAsync();
                        if (Vdo != null)
                        {
                            proImgVideo.VideoUrl = Vdo.FileUrl;
                            proImgVideo.ImageUrl = "-";
                            proImgVideo.Id = item.Id;
                            proImgVideo.ProductId = item.ProductId;
                            proImgVideo.IsDefault = item.IsDefault;
                            proImgVideos.Add(proImgVideo);

                        }
                        else
                        {
                            continue;
                        }
                    }
                }

            }
            return proImgVideos;
        }

        public async Task<ProductImageAndVideoDTO> GetProductImagesVideoById(int id)
        {
            var proImgVideo = new ProductImageAndVideoDTO();
            var productImg = await _context.ProductImages.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (productImg != null)
            {
                var img = await _context.FileManager.Where(x => x.Id == productImg.ImageMdId).FirstOrDefaultAsync();

                proImgVideo = new ProductImageAndVideoDTO();
                if (img != null)
                {
                    proImgVideo.ImageUrl = img.FileUrl;
                    proImgVideo.Id = productImg.Id;
                    proImgVideo.ProductId = productImg.ProductId;
                    proImgVideo.IsDefault = productImg.IsDefault;
                    proImgVideo.VideoUrl = "-";

                }
                else
                {
                    var Vdo = await _context.FileManager.Where(x => x.Id == productImg.VideoId).FirstOrDefaultAsync();
                    if (Vdo != null)
                    {
                        proImgVideo.VideoUrl = Vdo.FileUrl;
                        proImgVideo.ImageUrl = "-";
                        proImgVideo.Id = productImg.Id;
                        proImgVideo.ProductId = productImg.ProductId;
                        proImgVideo.IsDefault = productImg.IsDefault;
                    }
                }

            }
            return proImgVideo;
        }

        public async Task<IEnumerable<ProductMasterDTO>> GetProductPendingList()
        {
            var pendingProducts = await (
                from product in _context.Product
                where product.UploadStatus == SD.Pending
                join proMstr in _context.ProductMaster on product.ProductKey equals proMstr.ProductKey
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
                select new
                {
                    proMstr.Id,
                    proMstr.ProductKey,
                    Product = product,
                    User = usr,
                    Karat = krt,
                    Category = cat,
                    Color = color,
                    Shape = shape,
                    Clarity = clarity,
                    CenterCarat = size,
                    CenterShape = shape,
                    AccentShape = ashape
                }).ToListAsync();

            var grouped = pendingProducts
                .GroupBy(x => new { x.Id, x.ProductKey, x.Category })
                .Select(g => new ProductMasterDTO
                {
                    Id = g.Key.Id,
                    ProductKey = g.Key.ProductKey,
                    CategoryId = g.Key.Category.Id,
                    CategoryName = g.Key.Category.Name,
                    ProductItems = g.Select(x => new ProductDTO
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        BandWidth = x.Product.BandWidth,
                        Length = x.Product.Length,
                        CaratName = x.Product.Carat,
                        CategoryId = x.Category?.Id,
                        CategoryName = x.Category?.Name,
                        ColorId = x.Color?.Id,
                        ColorName = x.Color?.Name,
                        ClarityId = x.Clarity?.Id,
                        ClarityName = x.Clarity?.Name,
                        ShapeName = x.Shape?.Name,
                        CenterShapeName = x.Shape?.Name,
                        UnitPrice = x.Product.UnitPrice,
                        Price = x.Product.Price,
                        IsActivated = x.Product.IsActivated,
                        CaratSizeId = x.Product.CaratSizeId,
                        Description = x.Product.Description,
                        Sku = x.Product.Sku,
                        ProductType = x.Category?.ProductType,
                        VenderName = x.Product.Vendor,
                        Grades = x.Product.Grades,
                        GoldWeight = x.Product.GoldWeight,
                        IsReadyforShip = x.Product.IsReadyforShip,
                        VenderStyle = x.Product.VenderStyle,
                        CenterCaratId = x.CenterCarat?.Id ?? 0,
                        CenterCaratName = x.CenterCarat?.Name,
                        CenterShapeId = x.Shape?.Id ?? 0,
                        Quantity = x.Product.Quantity,
                        KaratId = x.Karat?.Id ?? 0,
                        Karat = x.Karat?.Name,
                        UploadStatus = x.Product.UploadStatus,
                        ProductDate = x.Product.UpdatedDate,
                        Diameter = x.Product.Diameter,
                        CTW = x.Product.CTW,
                        Certificate = x.Product.Certificate,
                        WholesaleCost = x.Product.WholesaleCost,
                        MMSize = x.Product.MMSize,
                        DiaWT = x.Product.DiaWT,
                        NoOfStones = x.Product.NoOfStones,
                        AccentStoneShapeName = x.AccentShape?.Name,
                        AccentStoneShapeId = x.Product.AccentStoneShapeId,
                        CreatedBy = x.Product.CreatedBy,
                        UpdatedBy = x.Product.UpdatedBy,
                        CreatedDate = x.Product.CreatedDate,
                        UpdatedDate = x.Product.UpdatedDate,
                        DisplayDate = x.Product.UpdatedDate?.ToString("dd/MM/yyyy hh:mm tt"),
                        UpdatedPersonName = x.User.FirstName,
                        ProductKey = x.Product.ProductKey,
                        GroupId = x.Product.GroupId
                    }).ToList()
                })
                .OrderByDescending(x => x.ProductItems.First().Sku)
                .ToList();

            // Collect all SKUs and Product IDs to batch query dependent data
            var allSkus = grouped.SelectMany(g => g.ProductItems.Select(p => p.Sku)).Distinct().ToList();
            var allProductIds = grouped.Select(g => g.ProductItems.First().ProductKey.ToString()).ToList();
            var allColorIds = grouped.SelectMany(g => g.ProductItems.Select(p => p.ColorId)).Distinct().ToList();
            var allCenterShapeIds = grouped.SelectMany(g => g.ProductItems.Select(p => p.CenterShapeId)).Distinct().ToList();

            // Load ProductImages and FileManager data in one go
            var imageData = await _context.ProductImages
                .Where(img => allProductIds.Contains(img.ProductId))
                .ToListAsync();

            var allFileIds = imageData.Select(i => i.ImageSmId)
                .Concat(imageData.Select(i => i.VideoId))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var fileManagerData = await _context.FileManager
                .Where(f => allFileIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.FileUrl);

            // Load ProductProperties by type
            var metalParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.Metal)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var caratSizeParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.CaratSize)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var shapeParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.Shape)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var allProperties = await _context.ProductProperty.ToListAsync();

            var metals = allProperties
                .Where(p => p.ParentId == metalParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    SymbolName = p.SymbolName,
                    IsActive = p.IsActive ?? false
                }).ToList();

            var caratSizes = allProperties
                .Where(p => p.ParentId == caratSizeParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsActive = p.IsActive ?? false
                }).ToList();

            var shapes = allProperties
                .Where(p => p.ParentId == shapeParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    IconPath = p.IconPath,
                    IsActive = p.IsActive ?? false
                }).ToList();

            // Now map everything back to each item
            foreach (var master in grouped)
            {
                foreach (var item in master.ProductItems)
                {
                    item.Metals = metals.Where(m => m.Id == item.ColorId).ToList();
                    item.CaratSizes = caratSizes.Where(c => c.Id == item.CenterCaratId).ToList();
                    item.Shapes = shapes.Where(s => s.Id == item.CenterShapeId).ToList();

                    item.ProductImageVideos = imageData
                        .Where(img => img.ProductId == item.ProductKey.ToString()
                                   && img.MetalId == item.ColorId
                                   && img.ShapeId == item.CenterShapeId)
                        .Select(img => new ProductImageAndVideoDTO
                        {
                            ProductId = img.ProductId,
                            ImageUrl = fileManagerData.ContainsKey(img.ImageSmId ?? 0) ? fileManagerData[img.ImageSmId.Value] : null,
                            VideoUrl = fileManagerData.ContainsKey(img.VideoId ?? 0) ? fileManagerData[img.VideoId.Value] : null,
                            IsDefault = img.IsDefault
                        }).ToList();
                }
            }

            return grouped;
        }

        public async Task<IEnumerable<ProductMasterDTO>> GetProductStyleList()
        {
            var pendingProducts = await (
                from product in _context.Product
                where product.UploadStatus == SD.Activated
                join proMstr in _context.ProductMaster on product.ProductKey equals proMstr.ProductKey
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
                select new
                {
                    proMstr.Id,
                    proMstr.ProductKey,
                    Product = product,
                    User = usr,
                    Karat = krt,
                    Category = cat,
                    Color = color,
                    Shape = shape,
                    Clarity = clarity,
                    CenterCarat = size,
                    CenterShape = shape,
                    AccentShape = ashape
                }).ToListAsync();

            var grouped = pendingProducts
                .GroupBy(x => new { x.Id, x.ProductKey, x.Category })
                .Select(g => new ProductMasterDTO
                {
                    Id = g.Key.Id,
                    ProductKey = g.Key.ProductKey,
                    CategoryId = g.Key.Category.Id,
                    CategoryName = g.Key.Category.Name,
                    ProductItems = g.Select(x => new ProductDTO
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        BandWidth = x.Product.BandWidth,
                        Length = x.Product.Length,
                        CaratName = x.Product.Carat,
                        CategoryId = x.Category?.Id,
                        CategoryName = x.Category?.Name,
                        ColorId = x.Color?.Id,
                        ColorName = x.Color?.Name,
                        ClarityId = x.Clarity?.Id,
                        ClarityName = x.Clarity?.Name,
                        ShapeName = x.Shape?.Name,
                        CenterShapeName = x.Shape?.Name,
                        UnitPrice = x.Product.UnitPrice,
                        Price = x.Product.Price,
                        IsActivated = x.Product.IsActivated,
                        CaratSizeId = x.Product.CaratSizeId,
                        Description = x.Product.Description,
                        Sku = x.Product.Sku,
                        ProductType = x.Category?.ProductType,
                        VenderName = x.Product.Vendor,
                        Grades = x.Product.Grades,
                        GoldWeight = x.Product.GoldWeight,
                        IsReadyforShip = x.Product.IsReadyforShip,
                        VenderStyle = x.Product.VenderStyle,
                        CenterCaratId = x.CenterCarat?.Id ?? 0,
                        CenterCaratName = x.CenterCarat?.Name,
                        CenterShapeId = x.Shape?.Id ?? 0,
                        Quantity = x.Product.Quantity,
                        KaratId = x.Karat?.Id ?? 0,
                        Karat = x.Karat?.Name,
                        UploadStatus = x.Product.UploadStatus,
                        ProductDate = x.Product.UpdatedDate,
                        Diameter = x.Product.Diameter,
                        CTW = x.Product.CTW,
                        Certificate = x.Product.Certificate,
                        WholesaleCost = x.Product.WholesaleCost,
                        MMSize = x.Product.MMSize,
                        DiaWT = x.Product.DiaWT,
                        NoOfStones = x.Product.NoOfStones,
                        AccentStoneShapeName = x.AccentShape?.Name,
                        AccentStoneShapeId = x.Product.AccentStoneShapeId,
                        CreatedBy = x.Product.CreatedBy,
                        UpdatedBy = x.Product.UpdatedBy,
                        CreatedDate = x.Product.CreatedDate,
                        UpdatedDate = x.Product.UpdatedDate,
                        DisplayDate = x.Product.UpdatedDate?.ToString("dd/MM/yyyy hh:mm tt"),
                        UpdatedPersonName = x.User.FirstName,
                        ProductKey = x.Product.ProductKey,
                        GroupId = x.Product.GroupId
                    }).ToList()
                })
                .OrderByDescending(x => x.ProductItems.First().Sku)
                .ToList();

            // Collect all SKUs and Product IDs to batch query dependent data
            var allSkus = grouped.SelectMany(g => g.ProductItems.Select(p => p.Sku)).Distinct().ToList();
            var allProductIds = grouped.Select(g => g.ProductItems.First().ProductKey.ToString()).ToList();
            var allColorIds = grouped.SelectMany(g => g.ProductItems.Select(p => p.ColorId)).Distinct().ToList();
            var allCenterShapeIds = grouped.SelectMany(g => g.ProductItems.Select(p => p.CenterShapeId)).Distinct().ToList();

            // Load ProductImages and FileManager data in one go
            var imageData = await _context.ProductImages
                .Where(img => allProductIds.Contains(img.ProductId))
                .ToListAsync();

            var allFileIds = imageData.Select(i => i.ImageSmId)
                .Concat(imageData.Select(i => i.VideoId))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var fileManagerData = await _context.FileManager
                .Where(f => allFileIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.FileUrl);

            // Load ProductProperties by type
            var metalParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.Metal)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var caratSizeParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.CaratSize)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var shapeParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.Shape)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var allProperties = await _context.ProductProperty.ToListAsync();

            var metals = allProperties
                .Where(p => p.ParentId == metalParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    SymbolName = p.SymbolName,
                    IsActive = p.IsActive ?? false
                }).ToList();

            var caratSizes = allProperties
                .Where(p => p.ParentId == caratSizeParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsActive = p.IsActive ?? false
                }).ToList();

            var shapes = allProperties
                .Where(p => p.ParentId == shapeParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    IconPath = p.IconPath,
                    IsActive = p.IsActive ?? false
                }).ToList();

            // Now map everything back to each item
            foreach (var master in grouped)
            {
                foreach (var item in master.ProductItems)
                {
                    item.Metals = metals.Where(m => m.Id == item.ColorId).ToList();
                    item.CaratSizes = caratSizes.Where(c => c.Id == item.CenterCaratId).ToList();
                    item.Shapes = shapes.Where(s => s.Id == item.CenterShapeId).ToList();

                    item.ProductImageVideos = imageData
                        .Where(img => img.ProductId == item.ProductKey.ToString()
                                   && img.MetalId == item.ColorId
                                   && img.ShapeId == item.CenterShapeId)
                        .Select(img => new ProductImageAndVideoDTO
                        {
                            ProductId = img.ProductId,
                            ImageUrl = fileManagerData.ContainsKey(img.ImageSmId ?? 0) ? fileManagerData[img.ImageSmId.Value] : null,
                            VideoUrl = fileManagerData.ContainsKey(img.VideoId ?? 0) ? fileManagerData[img.VideoId.Value] : null,
                            IsDefault = img.IsDefault
                        }).ToList();
                }
            }

            return grouped;
        }

        public async Task<IEnumerable<ProductMasterDTO>> GetProductHoldList()
        {
            var pendingProducts = await (
                from product in _context.Product
                where product.UploadStatus == SD.Hold
                join proMstr in _context.ProductMaster on product.ProductKey equals proMstr.ProductKey
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
                select new
                {
                    proMstr.Id,
                    proMstr.ProductKey,
                    Product = product,
                    User = usr,
                    Karat = krt,
                    Category = cat,
                    Color = color,
                    Shape = shape,
                    Clarity = clarity,
                    CenterCarat = size,
                    CenterShape = shape,
                    AccentShape = ashape
                }).ToListAsync();

            var grouped = pendingProducts
                .GroupBy(x => new { x.Id, x.ProductKey, x.Category })
                .Select(g => new ProductMasterDTO
                {
                    Id = g.Key.Id,
                    ProductKey = g.Key.ProductKey,
                    CategoryId = g.Key.Category.Id,
                    CategoryName = g.Key.Category.Name,
                    ProductItems = g.Select(x => new ProductDTO
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        BandWidth = x.Product.BandWidth,
                        Length = x.Product.Length,
                        CaratName = x.Product.Carat,
                        CategoryId = x.Category?.Id,
                        CategoryName = x.Category?.Name,
                        ColorId = x.Color?.Id,
                        ColorName = x.Color?.Name,
                        ClarityId = x.Clarity?.Id,
                        ClarityName = x.Clarity?.Name,
                        ShapeName = x.Shape?.Name,
                        CenterShapeName = x.Shape?.Name,
                        UnitPrice = x.Product.UnitPrice,
                        Price = x.Product.Price,
                        IsActivated = x.Product.IsActivated,
                        CaratSizeId = x.Product.CaratSizeId,
                        Description = x.Product.Description,
                        Sku = x.Product.Sku,
                        ProductType = x.Category?.ProductType,
                        VenderName = x.Product.Vendor,
                        Grades = x.Product.Grades,
                        GoldWeight = x.Product.GoldWeight,
                        IsReadyforShip = x.Product.IsReadyforShip,
                        VenderStyle = x.Product.VenderStyle,
                        CenterCaratId = x.CenterCarat?.Id ?? 0,
                        CenterCaratName = x.CenterCarat?.Name,
                        CenterShapeId = x.Shape?.Id ?? 0,
                        Quantity = x.Product.Quantity,
                        KaratId = x.Karat?.Id ?? 0,
                        Karat = x.Karat?.Name,
                        UploadStatus = x.Product.UploadStatus,
                        ProductDate = x.Product.UpdatedDate,
                        Diameter = x.Product.Diameter,
                        CTW = x.Product.CTW,
                        Certificate = x.Product.Certificate,
                        WholesaleCost = x.Product.WholesaleCost,
                        MMSize = x.Product.MMSize,
                        DiaWT = x.Product.DiaWT,
                        NoOfStones = x.Product.NoOfStones,
                        AccentStoneShapeName = x.AccentShape?.Name,
                        AccentStoneShapeId = x.Product.AccentStoneShapeId,
                        CreatedBy = x.Product.CreatedBy,
                        UpdatedBy = x.Product.UpdatedBy,
                        CreatedDate = x.Product.CreatedDate,
                        UpdatedDate = x.Product.UpdatedDate,
                        DisplayDate = x.Product.UpdatedDate?.ToString("dd/MM/yyyy hh:mm tt"),
                        UpdatedPersonName = x.User.FirstName,
                        ProductKey = x.Product.ProductKey,
                        GroupId = x.Product.GroupId
                    }).ToList()
                })
                .OrderByDescending(x => x.ProductItems.First().Sku)
                .ToList();

            // Collect all SKUs and Product IDs to batch query dependent data
            var allSkus = grouped.SelectMany(g => g.ProductItems.Select(p => p.Sku)).Distinct().ToList();
            var allProductIds = grouped.Select(g => g.ProductItems.First().ProductKey.ToString()).ToList();
            var allColorIds = grouped.SelectMany(g => g.ProductItems.Select(p => p.ColorId)).Distinct().ToList();
            var allCenterShapeIds = grouped.SelectMany(g => g.ProductItems.Select(p => p.CenterShapeId)).Distinct().ToList();

            // Load ProductImages and FileManager data in one go
            var imageData = await _context.ProductImages
                .Where(img => allProductIds.Contains(img.ProductId))
                .ToListAsync();

            var allFileIds = imageData.Select(i => i.ImageSmId)
                .Concat(imageData.Select(i => i.VideoId))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var fileManagerData = await _context.FileManager
                .Where(f => allFileIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.FileUrl);

            // Load ProductProperties by type
            var metalParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.Metal)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var caratSizeParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.CaratSize)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var shapeParentId = await _context.ProductProperty
                .Where(p => p.Name == SD.Shape)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            var allProperties = await _context.ProductProperty.ToListAsync();

            var metals = allProperties
                .Where(p => p.ParentId == metalParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    SymbolName = p.SymbolName,
                    IsActive = p.IsActive ?? false
                }).ToList();

            var caratSizes = allProperties
                .Where(p => p.ParentId == caratSizeParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsActive = p.IsActive ?? false
                }).ToList();

            var shapes = allProperties
                .Where(p => p.ParentId == shapeParentId)
                .Select(p => new ProductPropertyDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    IconPath = p.IconPath,
                    IsActive = p.IsActive ?? false
                }).ToList();

            // Now map everything back to each item
            foreach (var master in grouped)
            {
                foreach (var item in master.ProductItems)
                {
                    item.Metals = metals.Where(m => m.Id == item.ColorId).ToList();
                    item.CaratSizes = caratSizes.Where(c => c.Id == item.CenterCaratId).ToList();
                    item.Shapes = shapes.Where(s => s.Id == item.CenterShapeId).ToList();

                    item.ProductImageVideos = imageData
                        .Where(img => img.ProductId == item.ProductKey.ToString()
                                   && img.MetalId == item.ColorId
                                   && img.ShapeId == item.CenterShapeId)
                        .Select(img => new ProductImageAndVideoDTO
                        {
                            ProductId = img.ProductId,
                            ImageUrl = fileManagerData.ContainsKey(img.ImageSmId ?? 0) ? fileManagerData[img.ImageSmId.Value] : null,
                            VideoUrl = fileManagerData.ContainsKey(img.VideoId ?? 0) ? fileManagerData[img.VideoId.Value] : null,
                            IsDefault = img.IsDefault
                        }).ToList();
                }
            }

            return grouped;
        }
    }
}
