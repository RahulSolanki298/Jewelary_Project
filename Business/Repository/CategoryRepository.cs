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

        public async Task<Category> SaveCategory(Category category)
        {
            try
            {
                if (category != null && category.Id > 0)
                {
                    var categoryDT = await _context.Category.FirstOrDefaultAsync(x => x.Id == category.Id);
                    categoryDT.Name = !string.IsNullOrEmpty(category.Name) ? category.Name : categoryDT.Name;
                    categoryDT.ProductType = !string.IsNullOrEmpty(category.ProductType) ? category.ProductType : categoryDT.ProductType;
                    categoryDT.CategoryImage = !string.IsNullOrEmpty(category.CategoryImage) ? category.CategoryImage : categoryDT.CategoryImage;
                    categoryDT.DisplayOrder = category.DisplayOrder > 0 ? category.DisplayOrder : categoryDT.DisplayOrder;
                    categoryDT.SEO_Title = !string.IsNullOrEmpty(category.SEO_Title) ? category.SEO_Title : categoryDT.SEO_Title;
                    categoryDT.SEO_Url = !string.IsNullOrEmpty(category.SEO_Url) ? category.SEO_Url : categoryDT.SEO_Url;
                    categoryDT.IsActivated = category.IsActivated;
                    categoryDT.Prefix = !string.IsNullOrEmpty(category.Prefix) ? category.Prefix : categoryDT.Prefix;
                    _context.Category.Update(categoryDT);
                    _context.SaveChanges();

                    return categoryDT;
                }
                else
                {
                    var categoryDT = new Category();
                    categoryDT.Name = category.Name;
                    categoryDT.ProductType = category.ProductType;
                    categoryDT.CategoryImage = category.CategoryImage;
                    categoryDT.DisplayOrder = category.DisplayOrder;
                    categoryDT.SEO_Title = category.SEO_Title;
                    categoryDT.SEO_Url = category.SEO_Url;
                    categoryDT.IsActivated = category.IsActivated;
                    categoryDT.Prefix = category.Prefix;
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
