using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Business.Repository
{
    public class DiamondRepository : IDiamondRepository
    {
        private readonly ApplicationDBContext _context;

        public DiamondRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<DiamondAllDataDto> GetDiamondsAsync(DiamondFilters filters, int pageNumber, int pageSize)
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
                throw new Exception("An error occurred while fetching diamonds.", ex);
            }
        }


        public async Task<IEnumerable<DiamondData>> GetDiamondList()
        {
            try
            {
                var diamonds = await _context.DiamondData
                    .FromSqlRaw("EXEC SP_SelectAllDiamonds")
                    .ToListAsync();

                return diamonds;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DiamondHistory> GetDiamondByStoneId(string stoneId)
        {
            try
            {
                var diamonds = await _context.DiamondHistory.Where(x => x.StoneId == stoneId).FirstOrDefaultAsync();

                return diamonds;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DiamondData>> GetShapeWiseDiamondList()
        {
            try
            {
                // Step 1: Get all DiamondData from stored procedure
                var diamondDT = await _context.DiamondData
                    .FromSqlRaw("EXEC SP_SelectAllDiamonds")
                    .ToListAsync();

                // Step 2: Group by ShapeId and select the first diamond in each group
                var shapeWiseDiamonds = diamondDT
                                            .GroupBy(x => x.ShapeId)
                                            .Select(g => g.First()) // You can change to .OrderBy(...).First() if needed
                                            .ToList();

                return shapeWiseDiamonds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> BulkInsertDiamondsAsync(string jsonData, int historyId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData)) return false;

                var jsonParam = new SqlParameter("@JsonData", jsonData);
                var historyIdParam = new SqlParameter("@HistoryId", historyId);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC InsertDiamondsFromJson @JsonData, @HistoryId",
                    jsonParam, historyIdParam);

                return true;
            }
            catch (Exception ex)
            {
                // Optionally log ex.Message
                return false;
            }
        }

        public DiamondData GetDiamondById(int diamondId)
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

        public async Task<IEnumerable<DiamondFileUploadHistoryDTO>> GetDiamondFileUploadedHistories()
        {
            try
            {
                var result = await (from histroty in _context.DiamondFileUploadHistory
                              join usr in _context.ApplicationUser on histroty.UploadedBy equals usr.Id
                              select new DiamondFileUploadHistoryDTO
                              {
                                  Id=histroty.Id,
                                  Title=histroty.Title,
                                  UploadedPersonName=usr.FirstName+" "+usr.LastName,
                                  UploadedBy=histroty.UploadedBy,
                                  NoOfFailed=histroty.NoOfFailed,
                                  NoOfSuccess=histroty.NoOfSuccess,
                                  IsSuccess=histroty.IsSuccess,
                                  UploadedDate=histroty.UploadedDate
                              }).ToListAsync();
                             
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AddDiamondFileUploadedHistory(DiamondFileUploadHistory diamondFileUpload)
        {
            try
            {
                var history = new DiamondFileUploadHistory();
                await _context.DiamondFileUploadHistory.AddAsync(diamondFileUpload);
                await _context.SaveChangesAsync();

                return diamondFileUpload.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
