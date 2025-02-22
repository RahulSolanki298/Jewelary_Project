using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface ISubCategoryRepository
    {
        public Task<SubCategory> GetSubCategoryById(int id);
        public Task<IEnumerable<SubCategory>> GetSubCategoryList();
        public Task<SubCategory> SaveSubCategory(SubCategory subCategory, int scatId = 0);
        public Task<bool> DeleteSubCategoryById(int id);
    }
}
