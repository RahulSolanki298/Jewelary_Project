using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using B2C_ECommerce.IServices;
using Common;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using DataAccess.Entities;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using Business.Repository.IRepository;

namespace B2C_ECommerce.Services
{
    public class DiamondService : IDiamondService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDBContext _context;
        private readonly IDiamondRepository _diamondRepository;

        public DiamondService(IHttpClientFactory httpClientFactory, ApplicationDBContext context, IDiamondRepository diamondRepository)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _context = context;
            _diamondRepository = diamondRepository;
        }

        public async Task<DiamondAllDataDto> GetDiamondListByFilter(DiamondFilters filters, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                DiamondAllDataDto diamonData = new DiamondAllDataDto();
                // Construct the SQL parameters
                var parameters = new[]
                {
                    new SqlParameter("@Shapes", SqlDbType.NVarChar) { Value = filters.Shapes != null && filters.Shapes.Any() ? string.Join(",", filters.Shapes) : (object)DBNull.Value },
                    new SqlParameter("@Colors", SqlDbType.NVarChar) { Value = filters.Colors != null && filters.Colors.Any() ? string.Join(",", filters.Colors) : (object)DBNull.Value },
                    new SqlParameter("@FromCarat", SqlDbType.Decimal) { Value = filters.FromCarat ?? (object)DBNull.Value },
                    new SqlParameter("@ToCarat", SqlDbType.Decimal) { Value = filters.ToCarat ?? (object)DBNull.Value },
                    new SqlParameter("@FromPrice", SqlDbType.Decimal) { Value = filters.FromPrice ?? (object)DBNull.Value },
                    new SqlParameter("@ToPrice", SqlDbType.Decimal) { Value = filters.ToPrice ?? (object)DBNull.Value },
                    new SqlParameter("@Cuts", SqlDbType.NVarChar) { Value = filters.Cuts != null && filters.Cuts.Any() ? string.Join(",", filters.Cuts) : (object)DBNull.Value },
                    new SqlParameter("@Clarities", SqlDbType.NVarChar) { Value = filters.Clarities != null && filters.Clarities.Any() ? string.Join(",", filters.Clarities) : (object)DBNull.Value },
                    new SqlParameter("@FromRatio", SqlDbType.Decimal) { Value = filters.FromRatio ?? (object)DBNull.Value },
                    new SqlParameter("@ToRatio", SqlDbType.Decimal) { Value = filters.ToRatio ?? (object)DBNull.Value },
                    new SqlParameter("@FromTable", SqlDbType.Decimal) { Value = filters.FromTable ?? (object)DBNull.Value },
                    new SqlParameter("@ToTable", SqlDbType.Decimal) { Value = filters.ToTable ?? (object)DBNull.Value },
                    new SqlParameter("@FromDepth", SqlDbType.Decimal) { Value = filters.FromDepth ?? (object)DBNull.Value },
                    new SqlParameter("@ToDepth", SqlDbType.Decimal) { Value = filters.ToDepth ?? (object)DBNull.Value },
                    new SqlParameter("@Polish", SqlDbType.NVarChar) { Value = filters.Polish != null && filters.Polish.Any() ? string.Join(",", filters.Polish) : (object)DBNull.Value },
                    new SqlParameter("@Fluor", SqlDbType.NVarChar) { Value = filters.Fluor != null && filters.Fluor.Any() ? string.Join(",", filters.Fluor) : (object)DBNull.Value },
                    new SqlParameter("@Symmeties", SqlDbType.NVarChar) { Value = filters.Symmeties != null && filters.Symmeties.Any() ? string.Join(",", filters.Symmeties) : (object)DBNull.Value },
                    new SqlParameter("@LabNames", SqlDbType.NVarChar) { Value = filters.LabNames != null && filters.LabNames.Any() ? string.Join(",", filters.LabNames) : (object)DBNull.Value },
                };

                diamonData.DiamondData = await _context.DiamondData
                    .FromSqlRaw("EXEC SP_GetDiamondDataBY_NEW_DiamondFilters @Shapes, @Colors, @FromCarat, @ToCarat, @FromPrice, @ToPrice, @Cuts, @Clarities, @FromRatio, @ToRatio, @FromTable, @ToTable, @FromDepth, @ToDepth, @Polish, @Fluor, @Symmeties,@LabNames", parameters)
                    .ToListAsync();

                diamonData.DiamondCounter = diamonData.DiamondData.Count();

                diamonData.DiamondData = diamonData.DiamondData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                return diamonData;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow
                throw new Exception("An error occurred while fetching diamonds.", ex);
            }
        }

        public async Task<DiamondData> GetDiamondById(int diamondId)
        {
            try
            {
                var diamond = _context.DiamondData
                    .FromSqlRaw($"EXEC SP_GetDiamondDataById {diamondId}")
                    .AsEnumerable()  // Forces LINQ operations to happen in-memory (on the client side)
                    .FirstOrDefault();

                return diamond;
            }
            catch (Exception ex)
            {
                // Optionally log the exception or handle it
                throw new Exception("An error occurred while fetching the diamond data.", ex);
            }
        }

        public async Task<IEnumerable<DiamondData>> GetSelectedDiamondByIds(int[] diamondIds)
        {
            var response = await _diamondRepository.GetDiamondList();

            var filteredDiamonds = response.Where(x => diamondIds.Contains(x.Id)).ToList();

            return filteredDiamonds;
        }

        public async Task<IEnumerable<DiamondShapeData>> GetShapeListAsync()
        {
            var diamondShapes = await _context.Diamonds
                                                .Select(x => x.ShapeId)
                                                .Distinct()
                                                .ToListAsync();

            var result = await _context.DiamondProperties
                .Where(par => diamondShapes.Contains(par.Id) && par.IsActivated)
                .Select(par => new DiamondShapeData
                {
                    Id = par.Id,
                    Name = par.Name,
                    ParentId = par.ParentId,
                    DispOrder = par.DispOrder,
                    IconPath = par.IconPath
                })
                .OrderBy(x => x.DispOrder)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetColorListAsync()
        {
            var diamondColorIds = await _context.Diamonds
                                                .Select(x => x.ColorId)
                                                .Distinct()
                                                .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
            var result = await _context.DiamondProperties
                .Where(par => diamondColorIds.Contains(par.Id) && par.IsActivated)
                .Select(par => new DiamondPropertyDTO
                {
                    Id = par.Id,
                    Name = par.Name,
                    ParentId = par.ParentId,
                    DispOrder = par.DispOrder,
                    IconPath = par.IconPath,
                    SymbolName = par.SymbolName
                })
                .OrderBy(x => x.DispOrder)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetCaratListAsync()
        {
            var result = await (from par in _context.DiamondProperties
                                join col in _context.DiamondProperties on par.ParentId equals col.Id
                                where col.Name == SD.CaratSize && par.IsActivated == true
                                select new DiamondPropertyDTO
                                {
                                    Id = par.Id,
                                    Name = par.Name,
                                    ParentId = col.Id,
                                    DispOrder = par.DispOrder,
                                    IconPath = par.IconPath,
                                    SymbolName = par.SymbolName
                                }).OrderBy(x => x.DispOrder).ToListAsync();
            return result;
        }


        public async Task<CaratSizeRanges> GetCaratSizeDataRangeAsync1()
        {
            var caratValues = await _context.Diamonds
                .Where(d => d.Carat != null)
                .Select(d => Convert.ToDecimal(d.Carat))
                .ToListAsync();

            if (!caratValues.Any())
                return new CaratSizeRanges(); // Return default or handle as needed

            var data= new CaratSizeRanges
            {
                MinCaratSize = caratValues.Min(),
                MaxCaratSize = caratValues.Max()
            };

            return data;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetCutListAsync()
        {
            var diamondCuts = await _context.Diamonds
                                                .Select(x => x.CutId)
                                                .Distinct()
                                                .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
            var result = await _context.DiamondProperties
                .Where(par => diamondCuts.Contains(par.Id) && par.IsActivated)
                .Select(par => new DiamondPropertyDTO
                {
                    Id = par.Id,
                    Name = par.Name,
                    ParentId = par.ParentId,
                    DispOrder = par.DispOrder,
                    IconPath = par.IconPath,
                    Description = par.Description,
                    IsActivated = par.IsActivated,
                    SymbolName = par.SymbolName
                })
                .OrderBy(x => x.DispOrder)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetClarityListAsync()
        {
            var diamondClarity = await _context.Diamonds
                                               .Select(x => x.ClarityId)
                                               .Distinct()
                                               .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
            var result = await _context.DiamondProperties
                .Where(par => diamondClarity.Contains(par.Id) && par.IsActivated)
                .Select(par => new DiamondPropertyDTO
                {
                    Id = par.Id,
                    Name = par.Name,
                    ParentId = par.ParentId,
                    DispOrder = par.DispOrder,
                    IconPath = par.IconPath,
                    Description = par.Description,
                    IsActivated = par.IsActivated,
                    SymbolName = par.SymbolName
                })
                .OrderBy(x => x.DispOrder)
                .ToListAsync();

            return result;
        }

        public async Task<TableRangeDTO> GetTableRangesAsync()
        {
            var data = await _context.Diamonds.ToListAsync();
            var tableRange = new TableRangeDTO();
            tableRange.MaxValue = data.Max(x => x.Table).Value;
            tableRange.MinValue = data.Min(x => x.Table).Value;

            return tableRange;
        }

        public async Task<DepthDTO> GetDepthRangesAsync()
        {
            var data = await _context.Diamonds.ToListAsync();
            var depthRange = new DepthDTO();
            depthRange.MaxValue = data.Max(x => x.Depth).Value;
            depthRange.MinValue = data.Min(x => x.Depth).Value;
            return depthRange;
        }

        public async Task<PriceRanges> GetProductPriceRangeData()
        {
            var data = await _context.Diamonds.ToListAsync();
            var priceRange = new PriceRanges();
            //priceRange.MaxPrice = data.Max(x => x.Amount).Value;
            //priceRange.MinPrice = data.Min(x => x.Amount).Value;

            priceRange.MaxPrice = data.Max(x => x.Price).Value;
            priceRange.MinPrice = data.Min(x => x.Price).Value;

            return priceRange;
        }

        public async Task<RatioDto> GetRatioRangesAsync()
        {
            var data = await _context.Diamonds.ToListAsync();
            var priceRange = new RatioDto();

            priceRange.MaxValue = data.Max(x => x.Ratio).Value;
            priceRange.MinValue = data.Min(x => x.Ratio).Value;

            return priceRange;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetFluorListAsync()
        {
            var diamondPolish = await _context.Diamonds
                                                .Select(x => x.FluorId)
                                                .Distinct()
                                                .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
            var result = await _context.DiamondProperties
                .Where(par => diamondPolish.Contains(par.Id) && par.IsActivated)
                .Select(par => new DiamondPropertyDTO
                {
                    Id = par.Id,
                    Name = par.Name,
                    ParentId = par.ParentId,
                    DispOrder = par.DispOrder,
                    IconPath = par.IconPath,
                    Description = par.Description,
                    IsActivated = par.IsActivated,
                    SymbolName = par.SymbolName
                })
                .OrderBy(x => x.DispOrder)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetPolishListAsync()
        {
            var diamondPolish = await _context.Diamonds
                                                .Select(x => x.PolishId)
                                                .Distinct()
                                                .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
            var result = await _context.DiamondProperties
                .Where(par => diamondPolish.Contains(par.Id) && par.IsActivated)
                .Select(par => new DiamondPropertyDTO
                {
                    Id = par.Id,
                    Name = par.Name,
                    ParentId = par.ParentId,
                    DispOrder = par.DispOrder,
                    IconPath = par.IconPath,
                    Description = par.Description,
                    IsActivated = par.IsActivated,
                    SymbolName = par.SymbolName
                })
                .OrderBy(x => x.DispOrder)
                .ToListAsync();

            return result;
        }


        public async Task<IEnumerable<DiamondPropertyDTO>> GetSymmetryListAsync()
        {
            var diamondSymm = await _context.Diamonds
                                                .Select(x => x.SymmetryId)
                                                .Distinct()
                                                .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
            var result = await _context.DiamondProperties
                .Where(par => diamondSymm.Contains(par.Id) && par.IsActivated)
                .Select(par => new DiamondPropertyDTO
                {
                    Id = par.Id,
                    Name = par.Name,
                    ParentId = par.ParentId,
                    DispOrder = par.DispOrder,
                    IconPath = par.IconPath,
                    Description = par.Description,
                    IsActivated = par.IsActivated,
                    SymbolName = par.SymbolName
                })
                .OrderBy(x => x.DispOrder)
                .ToListAsync();

            return result;
        }

    }
}
