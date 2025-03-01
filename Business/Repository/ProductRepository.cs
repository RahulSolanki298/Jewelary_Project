using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
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

                int colorId, subCategoryId, categoryId, clarityId, caratId, caratSizeId, shapeId = 0;

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

                    // Check if a product already exists based on related field IDs
                    existingProduct = await _context.Product
                        .Where(x => x.ProductType == product.ProductType
                                    && x.CategoryId == categoryId
                                    && x.SubCategoryId == subCategoryId
                                    && x.ClarityId == clarityId
                                    && x.ColorId == colorId
                                    && x.CaratId == caratId
                                    && x.CaratSizeId == caratSizeId)
                        .FirstOrDefaultAsync();

                    if (existingProduct != null)
                    {
                        // Update existing product
                        existingProduct.Title = $"{product.CategoryName} {product.ColorName} {product.CaratName} {product.ProductType}";
                        existingProduct.Sku = product.Sku;
                        existingProduct.Price = product.Price;
                        existingProduct.UnitPrice = product.UnitPrice;
                        existingProduct.Quantity = product.Quantity;

                        updateList.Add(existingProduct);
                    }
                    else
                    {
                        // Insert new product
                        newProduct = new Product
                        {
                            Title = $"{product.CategoryName} {product.ColorName} {product.CaratName} {product.ProductType}",
                            Sku = product.Sku,
                            CategoryId = categoryId,
                            SubCategoryId = subCategoryId,
                            CaratSizeId = caratSizeId,
                            CaratId = caratId,
                            ClarityId = clarityId,
                            ColorId = colorId,
                            Description=product.Description,
                            IsActivated=product.IsActivated,
                            StyleId=product.StyleId,
                            GoldWeight=product.GoldWeight,
                            GoldPurity=product.GoldPurity,
                            Price = product.Price,
                            UnitPrice = product.UnitPrice,
                            Quantity = product.Quantity,
                            ProductType = product.ProductType,
                            ShapeId = shapeId,
                            Id=product.Id
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



        public async Task<IEnumerable<ProductDTO>> GetProductList()
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
                                      StyleName = sty != null ? sty.Name : null,
                                      CollectionsId = product.CollectionsId,
                                      CollectionName = col != null ? col.HistoryTitle : null
                                  }).ToListAsync();

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

        
    }
}
