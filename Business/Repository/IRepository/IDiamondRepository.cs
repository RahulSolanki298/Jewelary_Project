using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IDiamondRepository
    {
        Task<IEnumerable<DiamondData>> GetDiamondsAsync(DiamondFilters filters, int pageNumber, int pageSize);

        Task<bool> BulkInsertDiamondsAsync(string jsonData);

        Task<IEnumerable<DiamondData>> GetDiamondList();

        DiamondData GetDiamondById(int diamondId);

        Task<IEnumerable<DiamondData>> GetShapeWiseDiamondList();
    }
}
