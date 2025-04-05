using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;
using Models;

namespace Business.Repository.IRepository
{
    public interface IDiamondPropertyRepository
    {
        Task<DiamondPropertyDTO> GetByIdAsync(int id);
        Task<IEnumerable<DiamondPropertyDTO>> GetAllAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetMetalListAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetCaratListAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetShapeListAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetClarityListAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetCutListAsync();
        Task<CaratSizeRanges> GetCaratSizeRangeAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetRatioListAsync();
        Task<PriceRanges> GetPriceRangeAsync();
        Task<DepthDTO> GetDepthRangeAsync();
        Task<TableRangeDTO> GetTableRangeAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetSymmetryListAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetFluorListAsync();
        Task<IEnumerable<DiamondPropertyDTO>> GetPolishListAsync();

        Task<bool> AddAsync(DiamondProperty entity);
        Task<bool> UpdateAsync(DiamondProperty entity);
        Task<bool> DeleteAsync(int id);
        Task<int> GetDiamondPropertyId(string diamondPropertyName, string properyName);
        Task<int> GetParentIdByName(string parentName);




    }
}
