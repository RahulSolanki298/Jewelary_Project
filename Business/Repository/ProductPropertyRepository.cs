using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Business.Repository
{
    public class ProductPropertyRepository : IProductPropertyRepository
    {
        public ApplicationDBContext _context;
        public ProductPropertyRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductProperty>> GetMainPropertyList() => await _context.ProductProperty.Where(x=>x.ParentId == null).ToListAsync();
        public async Task<IEnumerable<ProductProperty>> GetProductPropertyList() => await _context.ProductProperty.ToListAsync();

        public async Task<ProductProperty> GetProductPropertyById(int Id) => await _context.ProductProperty.FirstOrDefaultAsync(x=>x.Id==Id);
        
        public async Task<bool> DeleteProductProperty(int Id)
        {
            try
            {
                var reqest = await _context.ProductProperty.FirstOrDefaultAsync(x => x.Id == Id);
                if (reqest.Id > 0)
                {
                    _context.ProductProperty.Remove(reqest);
                    _context.SaveChanges();

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ProductProperty> SaveProductProperty(ProductProperty productProperty, int producutPropertyId = 0)
        {
            try
            {
                if (producutPropertyId > 0)
                {
                    var productDT = await _context.ProductProperty.FirstOrDefaultAsync(x => x.Id == producutPropertyId);
                    productDT.Name = productDT.Name;
                    productDT.CategoryId = productProperty.CategoryId;
                    productDT.Description = productProperty.Description;
                    if (productProperty.ParentId != null)
                    {
                        productDT.ParentId = productProperty.ParentId;
                    }

                    _context.ProductProperty.Update(productDT);
                    _context.SaveChanges();

                    return productDT;
                }
                else
                {
                    var productDT = new ProductProperty();
                    productDT.Name = productProperty.Name;
                    productDT.CategoryId = productProperty.CategoryId;
                    productDT.Description = productProperty.Description;

                    if (productProperty.ParentId != null)
                    {
                        productDT.ParentId = productProperty.ParentId;
                        
                    }
                    await _context.ProductProperty.AddAsync(productDT);
                    await _context.SaveChangesAsync();
                    return productDT;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IEnumerable<ProductPropertyDTO>> GetProductColorList()
        {
            try
            {

            
            var colors = await (from prd in _context.ProductProperty
                                join met in _context.ProductProperty.Where(x => x.Name == SD.Metal && x.IsActive==true) on prd.ParentId equals met.Id
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

        public async Task<IEnumerable<ProductPropertyDTO>> GetProductCaratSizeList()
        {
            var colors = await (from prd in _context.ProductProperty
                                join met in _context.ProductProperty.Where(x => x.Name == SD.CaratSize && x.IsActive == true) on prd.ParentId equals met.Id
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


        public async Task<IEnumerable<CategoryDTO>> GetProductCategoryList()
        {
            var categories = await (from prd in _context.Category
                                    select new CategoryDTO
                                    {
                                        Id = prd.Id,
                                        Name = prd.Name,
                                    }).ToListAsync();

            return categories;
        }


        public async Task<IEnumerable<CategoryDTO>> GetProductSubCategoryList()
        {
            var categories = await (from prd in _context.SubCategory
                                    select new CategoryDTO
                                    {
                                        Id = prd.Id,
                                        Name = prd.Name,
                                    }).ToListAsync();

            return categories;
        }


        public async Task<IEnumerable<ProductCollectionDTO>> GetCategories()
        {
            var collections = await (from prd in _context.ProductCollections
                                    select new ProductCollectionDTO
                                    {
                                        Id = prd.Id,
                                        CollectionName = prd.CollectionName,
                                    }).ToListAsync();

            return collections;
        }

        public async Task<IEnumerable<ProductStyleDTO>> GetSubCategoryList()
        {
            var categories = await (from prd in _context.ProductStyles
                                    select new ProductStyleDTO
                                    {
                                        Id = prd.Id,
                                        StyleName = prd.StyleName,
                                    }).ToListAsync();

            return categories;
        }


    }
}
