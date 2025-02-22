using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Business.Repository
{
    public class CategoryRepository : ICategoryRepositry
    {
        public ApplicationDBContext _context;
        public CategoryRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteCategoryById(int id)
        {
            try
            {
                var reqest = await _context.Category.FirstOrDefaultAsync(x => x.Id == id);
                if (reqest.Id > 0)
                {
                    _context.Category.Remove(reqest);
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

        public async Task<Category> GetCategoryById(int id) => await _context.Category.FindAsync(id);

        public async Task<IEnumerable<Category>> GetCategoryList() => await _context.Category.ToListAsync();

        public async Task<Category> SaveCategory(Category category, int catId = 0)
        {
            try
            {
                if (catId > 0)
                {
                    var categoryDT = await _context.Category.FirstOrDefaultAsync(x => x.Id == catId);
                    categoryDT.Name = category.Name;
                    categoryDT.ProductType = category.ProductType;

                    _context.Category.Update(categoryDT);
                    _context.SaveChanges();

                    return categoryDT;
                }
                else
                {
                    var categoryDT = new Category();
                    categoryDT.Name = category.Name;
                    categoryDT.ProductType = category.ProductType;
                    await _context.Category.AddAsync(categoryDT);
                    await _context.SaveChangesAsync();
                    return categoryDT;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
