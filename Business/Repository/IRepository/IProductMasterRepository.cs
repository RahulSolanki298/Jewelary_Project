using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IProductMasterRepository
    {
        Task<List<ProductMaster>> GetProductMasterList();
    }
}
