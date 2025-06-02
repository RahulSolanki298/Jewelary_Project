using DataAccess.Entities;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IProductPropertyRepository
    {
        Task<IEnumerable<ProductPropertyDTO>> GetMainPropertyList();
        Task<IEnumerable<ProductProperty>> GetProductPropertyList();
        Task<ProductProperty> GetProductPropertyById(int Id);
        Task<bool> DeleteProductProperty(int Id);
        Task<ProductProperty> SaveProductProperty(ProductProperty productProperty, int producutPropertyId = 0);

        Task<IEnumerable<ProductPropertyDTO>> GetProductColorList();

        Task<IEnumerable<ProductPropertyDTO>> GetProductCaratSizeList();

        Task<IEnumerable<ProductCollectionDTO>> GetCategories();

        Task<IEnumerable<ProductStyleDTO>> GetSubCategoryList();

        Task<IEnumerable<CategoryDTO>> GetProductCategoryList();

        Task<IEnumerable<SubCategoryDTO>> GetProductSubCategoryList();

        Task<IEnumerable<ProductPropertyDTO>> GetProductShapeList();


        Task<PriceRanges> GetPriceRangeAsync();

        Task<IEnumerable<ProductPriceDTO>> GetProductPrices();


        Task<IEnumerable<ProductPropertyDTO>> GetKaratList();


        Task<IEnumerable<ProductPropertyDTO>> GetPropertyItemsByName(string propertyName);

    }
}
