using DataAccess.Entities;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IProductStyleRepository
    {
        Task<List<ProductStyles>> GetProductStyles();

        Task<ProductStyles> GetProductStyleById(int id);

        Task<List<ProductStyles>> GetProductStyleByCategoryId(int categoryId);

        Task<bool> DeleteProductStyle(int styleId);
        
        Task<bool> SaveProductStyle(ProductStyleDTO product);
    }
}
