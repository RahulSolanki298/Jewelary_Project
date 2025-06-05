using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IBlogRepository
    {
        Task<Blogs> GetBlogById(int id);

        Task<List<Blogs>> GetBlogList();

        Task<Blogs> SaveBlogAsync(Blogs blogs);

        Task<bool> RemoveBlog(int id);

        Task<List<string>> GetBlogCategoryList();
    }
}
