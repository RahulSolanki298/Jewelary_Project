using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
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

        public async Task<bool> SaveProductList(List<ProductDTO> products)
        {
            try
            {
                // Step 0: Initilize veriable
                var newProduct = new Product();
                var existingProduct = new Product();
                int styleId = 0;

                var styleDT = new ProductStyles();

                // Step 1: Fetch all related entities in bulk to avoid repeated database calls
                var colors = await GetColorList();
                var categories = await _context.Category.ToListAsync();
                var subCategories = await _context.SubCategory.ToListAsync();
                var clarities = await GetClarityList();
                var carats = await GetCaratList();
                var caratSizes = await _context.ProductCaratSize.ToListAsync();
                var shapes = await GetShapeList();
                var goldWeight = await GetGoldPurityList();
                var goldPurity = await GetGoldPurityList();

                // Step 2: Create dictionaries for fast lookup by Name
                var colorDict = colors.ToDictionary(x => x.Name, x => x.Id);
                var categoryDict = categories.ToDictionary(x => x.Name, x => x.Id);
                var subCategoryDict = subCategories.ToDictionary(x => x.Name, x => x.Id);
                var clarityDict = clarities.ToDictionary(x => x.Name, x => x.Id);
                var caratDict = carats.ToDictionary(x => x.Name, x => x.Id);
                var caratSizeDict = caratSizes.ToDictionary(x => x.Name, x => x.Id);
                var shapeDict = shapes.ToDictionary(x => x.Name, x => x.Id);
                var weightDict = shapes.ToDictionary(x => x.Name, x => x.Id);
                var purityDict = shapes.ToDictionary(x => x.Name, x => x.Id);

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
                    goldWeightId = weightDict.GetValueOrDefault(product.GoldWeight);
                    goldPurityId = purityDict.GetValueOrDefault(product.GoldPurity);

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
                        existingProduct.Title = $"{product.CategoryName} {product.ColorName} {product.CaratName} {product.ProductType} {product.Sku}";
                        existingProduct.Sku = product.Sku;
                        existingProduct.Price = product.Price;
                        existingProduct.UnitPrice = product.UnitPrice;
                        existingProduct.Quantity = product.Quantity;
                        existingProduct.StyleId = styleId;
                        existingProduct.IsActivated = product.IsActivated;
                        updateList.Add(existingProduct);
                    }
                    else
                    {
                        // Insert new product
                        newProduct = new Product
                        {
                            Title = $"{product.CategoryName} {product.ColorName} {product.CaratName} {product.ProductType} {product.Sku}",
                            Sku = product.Sku,
                            CategoryId = categoryId,
                            SubCategoryId = subCategoryId,
                            CaratSizeId = caratSizeId,
                            CaratId = caratId,
                            ClarityId = clarityId,
                            Length = product.Length,
                            ColorId = colorId,
                            Description = product.Description,
                            IsActivated = product.IsActivated,
                            StyleId = styleId,
                            GoldWeightId = goldWeightId,
                            GoldPurityId = goldPurityId,
                            Price = product.Price,
                            UnitPrice = product.UnitPrice,
                            Quantity = product.Quantity,
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
                        existingProduct.Price = product.Price;
                        existingProduct.UnitPrice = product.UnitPrice;
                        existingProduct.Quantity = product.Quantity;
                        existingProduct.IsActivated = product.IsActivated;
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
                            IsActivated = product.IsActivated,
                            //GoldWeight = product.GoldWeight,
                            //GoldPurity = product.GoldPurity,
                            Price = product.Price,
                            UnitPrice = product.UnitPrice,
                            Quantity = product.Quantity,
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
                        existingProduct.ProductDate = product.ProductDate;
                        existingProduct.Designer = product.Designer;
                        existingProduct.CadDesigner = product.CadDesigner;
                        existingProduct.Remarks = product.Remarks;
                        existingProduct.Carat = product.CaratName;
                        existingProduct.Gender = product.Gender;
                        existingProduct.MfgDesign = product.MfgDesign;
                        existingProduct.Package = product.Package;
                        existingProduct.Occasion = product.Occasion;
                        existingProduct.ParentDesign = product.ParentDesign;
                        existingProduct.IsActivated = product.IsActivated;
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
                            ProductDate = product.ProductDate,
                            Designer = product.Designer,
                            CadDesigner = product.CadDesigner,
                            Remarks = product.Remarks,
                            Carat = product.CaratName,
                            Gender = product.Gender,
                            Package = product.Package,
                            Occasion = product.Occasion,
                            IsActivated = product.IsActivated,
                            CollectionsId = product.CollectionsId,
                            ParentDesign = product.ParentDesign,
                            ProductType = product.ProductType,
                            MfgDesign = product.MfgDesign,
                            Vendor = product.VenderName,
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

        public async Task<IEnumerable<Product>> GetProductCollectionNewList()
        {
            var products = await _context.Product.ToListAsync();
            return products;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductStyleList()
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

                                  join sty in _context.ProductProperty on product.StyleId equals sty.Id into styleGroup
                                  from sty in styleGroup.DefaultIfEmpty() // LEFT JOIN

                                  join col in _context.CollectionHistory on product.CollectionsId equals col.Id into colGroup
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
                                      StyleId = product.StyleId,
                                      StyleName = sty != null ? sty.Name : null
                                  }).Where(x => !String.IsNullOrEmpty(x.StyleName) && x.IsActivated == true).ToListAsync();

            return products;

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
            var colorDT = await _context.ProductProperty.Where(static x => x.Name == "Metal").FirstOrDefaultAsync();
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
            var caratDT = await _context.ProductProperty.Where(static x => x.Name == "Carat").FirstOrDefaultAsync();
            return caratDT.Id;
        }

        public async Task<List<ProductProperty>> GetCaratList()
        {
            int caratId = await GetCaratId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == caratId).ToListAsync();
            return result;
        }

        public async Task<int> GetShapeId()
        {
            var shapeDT = await _context.ProductProperty.Where(static x => x.Name == "Shape").FirstOrDefaultAsync();
            return shapeDT.Id;
        }

        public async Task<List<ProductProperty>> GetShapeList()
        {
            int shapeId = await GetShapeId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == shapeId).ToListAsync();
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
            int weightId = await GetGoldWeightById();
            var result = await _context.ProductProperty.Where(x => x.ParentId == weightId).ToListAsync();
            return result;
        }

        public string ExtractStyleName(string fileName)
        {
            int underscoreIndex = fileName.IndexOf('_');
            return (underscoreIndex > 0) ? fileName.Substring(0, underscoreIndex) : "Unknown";
        }

    }
}
