using DataAccess.Entities;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IDiamondRepository
    {
        Task<DiamondAllDataDto> GetDiamondsAsync(DiamondFilters filters, int pageNumber, int pageSize);

        //Task<bool> BulkInsertDiamondsAsync(string jsonData);

        Task<IEnumerable<DiamondData>> GetDiamondList();

        DiamondData GetDiamondById(int diamondId);

        Task<IEnumerable<DiamondData>> GetShapeWiseDiamondList();

        Task<int> AddDiamondFileUploadedHistory(DiamondFileUploadHistory diamondFileUpload);

        Task<List<Diamond>> BulkInsertDiamondsAsync(string jsonData, int historyId);

        Task<IEnumerable<DiamondFileUploadHistoryDTO>> GetDiamondFileUploadedHistories();

        Task<bool> BulkInsertDiamondHistoryAsync(List<Diamond> data);

        Task<Diamond> GetDiamondByStoneId(string stoneId);

        Task<IEnumerable<DiamondData>> GetPendingDiamondList();
    }
}
