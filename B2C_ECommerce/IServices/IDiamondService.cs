using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IDiamondService
    {
        Task<List<DiamondData>> GetDiamondListByFilter(DiamondFilters diamondFilters, int pageNumber = 1, int pageSize = 10);
        Task<DiamondData> GetDiamondById(int diamondId);
    }
}
