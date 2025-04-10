using DataAccess.Entities;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IProductService
    {
        Task<List<Product>> GetProductListByFilter();


        Task<List<ProductPropertyDTO>> GetProductColorList();

        Task<List<ProductStyleDTO>> GetCategoriesList();

        Task<List<ProductCollectionDTO>> GetSubcategoryList();
    }
}
