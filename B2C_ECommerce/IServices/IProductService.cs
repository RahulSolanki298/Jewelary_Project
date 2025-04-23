using DataAccess.Entities;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetProductListByFilter(ProductFilters productFilters, int pageNumber = 1, int pageSize = 10);

        Task<List<ProductPropertyDTO>> GetProductColorList();

        Task<List<SubCategoryDTO>> GetSubcategoryList();

        Task<List<CategoryDTO>> GetCategoriesList();

        Task<ProductDTO> GetProductByProductId(string productId);

        Task<List<ProductPropertyDTO>> GetShapeList();

        Task<PriceRanges> GetProductPriceRangeData();

        Task<ProductDTO> GetProductsByColorId(string sku, int? colorId = 0);

        Task<ProductDTO> GetProductsByCaratId(string sku, int? caratId = 0);

    }
}
