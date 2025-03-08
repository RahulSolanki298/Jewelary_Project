using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;
using Models;

namespace Business.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<int> GetColorId();

        Task<List<ProductProperty>> GetColorList();

        Task<int> GetCaratId();

        Task<List<ProductProperty>> GetCaratList();

        Task<int> GetShapeId();

        Task<List<ProductProperty>> GetShapeList();

        Task<int> GetClarityId();

        Task<List<ProductProperty>> GetClarityList();

        Task<IEnumerable<ProductDTO>> GetProductCollectionList();

        Task<IEnumerable<ProductDTO>> GetProductStyleList();

        Task<bool> SaveProductList(List<ProductDTO> products);

        Task<bool> SaveProductCollectionList(List<ProductDTO> products);

        Task<int> GetGoldWeightById();

        Task<List<ProductProperty>> GetGoldWeightList();

        Task<int> GetGoldPurityById();

        Task<List<ProductProperty>> GetGoldPurityList();

        string ExtractStyleName(string fileName);
    }
}
