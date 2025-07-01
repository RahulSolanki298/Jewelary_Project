using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;
using Models;

namespace Business.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<ProductProperty>> GetColorList();
        Task<List<ProductProperty>> GetCaratList();
        Task<List<ProductProperty>> GetShapeList();
        Task<List<ProductProperty>> GetClarityList();
        Task<IEnumerable<ProductDTO>> GetProductCollectionList();
        Task<IEnumerable<ProductMasterDTO>> GetProductStyleList();
        Task<bool> SaveProductList(List<ProductDTO> products);
        Task<bool> SaveProductCollectionList(List<ProductDTO> products);
        Task<List<ProductProperty>> GetGoldWeightList();
        Task<List<ProductProperty>> GetGoldPurityList();
        Task<bool> SaveImageVideoAsync(ProductImages ImgVdoData);
        Task<IEnumerable<Product>> GetProductCollectionNewList();
        Task<bool> SaveNewProductCollectionList(List<ProductDTO> products);
        Task<int> SaveImageVideoPath(string imgVdoPath);
        FileSplitDTO ExtractStyleName(string fileName);
        Task<Product> GetProductByDesignNo(string designNo, int metalId);
        Task<int> GetKaratId();
        Task<List<ProductProperty>> GetKaratList();
        Task<bool> UpdateProductDetailsByExcel(List<ProductDTO> products);
        Task<ProductDTO> GetProductWithDetails(string productId);
        Task<int> GetMetalId(string name);
        Task<List<Product>> GetProductDataByDesignNo(string designNo, int metalId);
        Task<ProductDTO> GetProductByColorId(string sku, int? colorId = 0, int? caratId = 0);
        Task<ProductProperty> GetKaratById(int karatId);
        Task<ProductPrices> GetProductPriceData(ProductPriceDTO price);
        Task<EventSites> GetEventSitesByName(string name);
        Task<ProductWeight> GetProductWeightData(ProductWeightDTO weightDTO);
        Task<int> AddProductFileUploadedHistory(ProductFileUploadHistory productFileUpload);
        Task<IEnumerable<ProductDTO>> GetProductUploadRequestList();
        Task<IEnumerable<ProductMasterDTO>> GetProductHoldList();
        Task<bool> UpdateProductStatus(string[] productIds, string status,string userId);
        Task<ProductImageAndVideoDTO> GetProductImagesVideoById(int id);
        //Task<ProductUploadResult> SaveNewProductList(List<ProductDTO> products, string categoryName, string userId, int fileHistoryId);
        Task<bool>  SaveEarringsList(List<ProductDTO> products, string categoryName, string userId, int fileHistoryId);
        Task<List<Product>> GetJewelleryDTByDesignNo(string designNo, int metalId, int shapeId);
        Task<ProductProperty> GetProductShapeId(string ShapeCode, int shapeId);
        Task<List<ProductImageAndVideoDTO>> GetProductImagesVideos(string productKey);

        Task<IEnumerable<ProductMasterDTO>> GetProductDeActivatedList();

        Task<IEnumerable<ProductMasterDTO>> GetProductPendingList(string status);
        Task<IEnumerable<ProductMasterDTO>> GetProductMasterList(string status);
        Task<ProductMstResponse> SaveNewProductListToDbAsync(List<ProductDTO> products, string categoryName, string userId, int fileHistoryId);
    }
}
