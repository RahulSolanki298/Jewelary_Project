using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface ICategoryRepositry
    {
        public Task<Category> GetCategoryById(int id);
        public Task<IEnumerable<Category>> GetCategoryList();
        public Task<Category> SaveCategory(Category category, int catId = 0);
        public Task<bool> DeleteCategoryById(int id);

    }
}
