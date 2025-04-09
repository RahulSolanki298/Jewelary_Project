using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IProductService
    {
        Task<List<Product>> GetProductListByFilter();
    }
}
