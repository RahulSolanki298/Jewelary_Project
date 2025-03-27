using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

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
    }
}
