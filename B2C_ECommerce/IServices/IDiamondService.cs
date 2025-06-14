using DataAccess.Entities;
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

        Task<IEnumerable<DiamondPropertyDTO>> GetColorListAsync();

        Task<TableRangeDTO> GetTableRangesAsync();

        Task<PriceRanges> GetProductPriceRangeData();

        Task<IEnumerable<DiamondPropertyDTO>> GetCaratListAsync();

        Task<DepthDTO> GetDepthRangesAsync();

        Task<IEnumerable<DiamondPropertyDTO>> GetClarityListAsync();

        Task<RatioDto> GetRatioRangesAsync();

        Task<IEnumerable<DiamondPropertyDTO>> GetFluorListAsync();

        Task<IEnumerable<DiamondPropertyDTO>> GetCutListAsync();

        Task<IEnumerable<DiamondPropertyDTO>> GetPolishListAsync();

        Task<IEnumerable<DiamondPropertyDTO>> GetSymmetryListAsync();

        Task<CaratSizeRanges> GetCaratSizeDataRangeAsync1();

    }
}
