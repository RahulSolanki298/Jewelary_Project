using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;
using Models;

namespace Business.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDTO>> GetProductList();

        Task<int> GetColorId();

        Task<List<ProductProperty>> GetColorList();

        Task<int> GetCaratId();

        Task<List<ProductProperty>> GetCaratList();

        Task<int> GetShapeId();

        Task<List<ProductProperty>> GetShapeList();

        Task<int> GetClarityId();

        Task<List<ProductProperty>> GetClarityList();

        Task<bool> SaveProductList(List<ProductDTO> products);
    }
}
