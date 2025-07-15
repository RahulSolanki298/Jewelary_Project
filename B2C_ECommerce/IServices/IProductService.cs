using DataAccess.Entities;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IProductService
    {
        //Task<IEnumerable<ProductMasterDTO>> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10);
        //Task<List<ProductDTO>> GetProductListByFilter(ProductFilters productFilters, int pageNumber = 1, int pageSize = 10);
        //Task<List<ProductDTO>> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10);
        //Task<List<ProductMasterDTO>> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10);

        Task<IEnumerable<ProductMasterDTO>> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10);
        Task<List<SubCategoryDTO>> GetSubcategoryList();
        Task<List<CategoryDTO>> GetCategoriesList();
        Task<ProductDTO> GetProductByProductId(string productId);
        Task<PriceRanges> GetProductPriceRangeData();
        Task<ProductDTO> GetProductsByColorId(string groupId, int? colorId = 0, int? caratId = 0);
        Task<List<ProductPropertyDTO>> GetProductColorList();
        Task<List<ProductPropertyDTO>> GetShapeList();
        Task<IEnumerable<ProductMasterDTO>> GetSelectedProductByIds(int[] productIds);
        Task<List<ProductMasterDTO>> GetJewelleryByShapeColorId(string sku, int colorId, int? shapeId = 0);
        Task<List<ProductStyles>> ProductStyleDataList();
        Task<List<ProductCollections>> ProductCollectionDataList();

        Task<List<ProductStyleDTO>> ProgramStylesList();
        Task<List<ProductCollectionDTO>> ProductCollectionList();

        /**/
        Task<IEnumerable<ProductMasterDTO>> GetProductStyleDTList();

        Task<IEnumerable<ProductMasterDTO>> GetProductMasterByProperty(string sku, int colorId, int shapeId);

        Task<IEnumerable<ProductMasterDTO>> GetProductListByGroupId(string groupId);
    }
}
