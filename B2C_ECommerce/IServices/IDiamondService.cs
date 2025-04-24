using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IDiamondService
    {
        Task<DiamondAllDataDto> GetDiamondListByFilter(DiamondFilters diamondFilters, int pageNumber = 1, int pageSize = 10);
        Task<DiamondData> GetDiamondById(int diamondId);

        Task<IEnumerable<DiamondShapeData>> GetShapeListAsync();

        Task<IEnumerable<DiamondData>> GetSelectedDiamondByIds(int[] diamondIds);
    }
}
