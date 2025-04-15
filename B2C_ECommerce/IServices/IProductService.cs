using DataAccess.Entities;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetProductListByFilter(ProductFilters productFilters);

        Task<List<ProductPropertyDTO>> GetProductColorList();

        Task<List<SubCategoryDTO>> GetSubcategoryList();

        Task<List<CategoryDTO>> GetCategoriesList();

        Task<ProductDTO> GetProductByProductId(string productId);

        Task<List<ProductPropertyDTO>> GetShapeList();

    }
}
