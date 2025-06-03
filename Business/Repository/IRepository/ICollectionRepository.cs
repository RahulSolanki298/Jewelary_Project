using DataAccess.Entities;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface ICollectionRepository
    {
        Task<List<ProductCollections>> GetProductCollections();
        Task<ProductCollections> GetProductCollectionById(int id);
        Task<IEnumerable<ProductDTO>> GetProductCollectionItemsList();
    }
}
