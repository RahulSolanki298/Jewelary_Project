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
        Task<bool> AddAsync(DiamondProperty entity);
        Task<bool> UpdateAsync(DiamondProperty entity);
        Task<bool> DeleteAsync(int id);
        Task<int> GetDiamondPropertyId(string diamondPropertyName, string properyName);
        Task<int> GetParentIdByName(string parentName);
    }
}
