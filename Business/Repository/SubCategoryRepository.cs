using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Repository
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly ApplicationDBContext _context;

        public SubCategoryRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Optimized Delete method with exception handling and SaveChangesAsync
        public async Task<bool> DeleteSubCategoryById(int id)
        {
            try
            {
                var subCategory = await _context.SubCategory.FindAsync(id);

                if (subCategory == null)
                {
                    return false; // Return false if not found
                }

                _context.SubCategory.Remove(subCategory);
                await _context.SaveChangesAsync(); // Asynchronous save

                return true;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                // Consider using a logging framework here (e.g., Serilog, NLog, etc.)
                throw new InvalidOperationException("Error occurred while deleting SubCategory.", ex);
            }
        }

        // Optimized Get method (Direct fetch without unnecessary async overhead)
        public async Task<IEnumerable<SubCategory>> GetSubCategoryList() =>
            await _context.SubCategory.ToListAsync(); 

        // Optimized GetById method (No need for extra query if null returned)
        public async Task<SubCategory> GetSubCategoryById(int id) =>
            await _context.SubCategory.FirstOrDefaultAsync(x => x.Id == id); // AsNoTracking

        // Optimized Save method with single async operation, better exception handling
        public async Task<SubCategory> SaveSubCategory(SubCategory subCategory, int scatId = 0)
        {
            try
            {
                var category = await _context.Category.FindAsync(subCategory.CategoryId);

                if (category == null)
                {
                    throw new ArgumentException("Category not found.");
                }

                if (scatId > 0)
                {
                    var existingSubCategory = await _context.SubCategory.FindAsync(subCategory.Id);
                    if (existingSubCategory == null)
                    {
                        throw new ArgumentException("SubCategory not found for update.");
                    }

                    // Update the existing SubCategory
                    existingSubCategory.Name = subCategory.Name;
                    existingSubCategory.CategoryId = category.Id;

                    _context.SubCategory.Update(existingSubCategory);
                }
                else
                {
                    // Insert new SubCategory
                    var newSubCategory = new SubCategory
                    {
                        Name = subCategory.Name,
                        CategoryId = category.Id
                    };
                    await _context.SubCategory.AddAsync(newSubCategory);
                }

                // Save changes asynchronously in one call
                await _context.SaveChangesAsync();

                return scatId > 0 ? subCategory : await _context.SubCategory
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name == subCategory.Name && x.CategoryId == category.Id);
            }
            catch (Exception ex)
            {
                // Log exception if needed
                throw new InvalidOperationException("Error occurred while saving SubCategory.", ex);
            }
        }
    }
}
