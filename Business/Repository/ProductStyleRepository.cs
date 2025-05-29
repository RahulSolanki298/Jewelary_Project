using Business.Repository.IRepository;
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
