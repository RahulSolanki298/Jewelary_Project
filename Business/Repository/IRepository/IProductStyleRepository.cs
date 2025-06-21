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
        Task<ProductStyles> GetProductStyleById(int id);

        Task<List<ProductStyles>> GetProductStyleByCategoryId(int categoryId);

        Task<List<ProductCollections>> GetProductCollectionsByCategoryId(int categoryId);

        Task<IEnumerable<ProductDTO>> GetProductStyleItemsList();

        Task<bool> DeleteProductStyle(int styleId);
        
        Task<bool> SaveProductStyle(ProductStyleDTO product);

        Task<bool> AddOrUpdateBulkProductStyle(BulkUpdateStatusRequest data);

        Task<List<ProductCollectionDTO>> GetProductCollections();

        Task<List<ProductStyleDTO>> GetProductStyles();
    }
}
