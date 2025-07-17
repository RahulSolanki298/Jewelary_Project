using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using Common;
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
        public DiamondRepository(ApplicationDBContext context) => _context = context;
        public async Task<DiamondAllDataDto> GetDiamondsAsync(DiamondFilters filters, int pageNumber, int pageSize)
        {
            DiamondAllDataDto diamonData = new DiamondAllDataDto();
            try
            {
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

                var diamondDT = await _context
                                    .Set<DiamondData>()
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
                var diamondResults = await _context
                                            .Set<DiamondData>()
                                            .FromSqlRaw("EXEC SP_SelectAllDiamonds")
                                            .ToListAsync();
                return diamondResults;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Diamond> GetDiamondByStoneId(string stoneId)
        {
            try
            {
                var diamonds = await _context.Diamonds.Where(x => x.StoneId == stoneId).FirstOrDefaultAsync();

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
                var diamondDT = await _context
                                    .Set<DiamondData>()
                                    .FromSqlRaw($"EXEC SP_SelectAllDiamonds")
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

        public async Task<List<Diamond>> BulkInsertDiamondsAsync(string jsonData, int historyId)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
                return new List<Diamond>();

            try
            {
                var jsonParam = new SqlParameter("@JsonData", SqlDbType.NVarChar)
                {
                    Value = jsonData
                };

                var historyIdParam = new SqlParameter("@HistoryId", SqlDbType.Int)
                {
                    Value = historyId
                };

                // Execute stored procedure
                var diamonds = await _context.Diamonds
                    .FromSqlRaw("EXEC [dbo].[InsertDiamondsFromJson] @JsonData, @HistoryId", jsonParam, historyIdParam)
                    .ToListAsync();

                return diamonds;
            }
            catch (Exception ex)
            {
                // Log or rethrow as needed
                Console.Error.WriteLine(ex);
                return new List<Diamond>();
            }
        }

        public async Task<bool> BulkInsertDiamondHistoryAsync(List<Diamond> data)
        {
            try
            {
                List<DiamondHistory> diamondHistories = new List<DiamondHistory>();

                foreach (var item in data)
                {
                    var diamondHist = new DiamondHistory
                    {
                        DiamondId = item.Id,
                        StoneId = item.StoneId,
                        DNA = item.DNA,
                        Step = item.Step,
                        TypeId = item.TypeId,
                        LabId = item.LabId,
                        ShapeId = item.ShapeId,
                        Carat = item.Carat,
                        ClarityId = item.ClarityId,
                        ColorId = item.ColorId,
                        CutId = item.CutId,
                        PolishId = item.PolishId,
                        SymmetryId = item.SymmetryId,
                        FluorId = item.FluorId,
                        RAP = item.RAP,
                        Discount = item.Discount,
                        Price = item.Price,
                        Amount = item.Amount,
                        Measurement = item.Measurement,
                        Ratio = item.Ratio,
                        Depth = item.Depth,
                        Table = item.Table,
                        Shade = item.Shade,
                        LabShape = item.LabShape,
                        RapAmount = item.RapAmount,
                        Certificate = item.Certificate,
                        DiamondImagePath = item.DiamondImagePath,
                        DiamondVideoPath = item.DiamondVideoPath,
                        UploadStatus = item.UploadStatus,
                        UpdatedBy = item.UpdatedBy,
                        UpdatedDate = item.UpdatedDate,
                        IsActivated = item.IsActivated,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate
                    };

                    diamondHistories.Add(diamondHist);
                }

                await _context.DiamondHistory.AddRangeAsync(diamondHistories);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Error in BulkInsertDiamondHistoryAsync: {ex.Message}");
                // Optionally log stack trace or use a logger
                return false;
            }
        }

        public DiamondData GetDiamondById(int diamondId)
        {
            try
            {
                var diamondResults =  _context
                                    .Set<DiamondData>()
                                    .FromSqlRaw($"EXEC SP_GetDiamondDataById {diamondId}")
                                    .AsEnumerable()
                                    .FirstOrDefault();

                return diamondResults;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the diamond data.", ex);
            }
        }


      

        public async Task<IEnumerable<DiamondData>> GetDiamondHistoryById(string diamondId)
        {
            try
            {
                var diamondResults = await _context
                                       .Set<DiamondData>()
                                       .FromSqlRaw($"EXEC SP_GetDiamondHistoryDataById {diamondId}")
                                       .ToListAsync();

                return diamondResults;

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
                                        Id = histroty.Id,
                                        Title = histroty.Title,
                                        UploadedPersonName = usr.FirstName + " " + usr.LastName,
                                        UploadedBy = histroty.UploadedBy,
                                        NoOfFailed = histroty.NoOfFailed,
                                        NoOfSuccess = histroty.NoOfSuccess,
                                        IsSuccess = histroty.IsSuccess,
                                        UploadedDate = histroty.UploadedDate
                                    }).ToListAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AddDiamondFileUploadedHistory(DiamondFileUploadHistoryDTO diamondFileUpload)
        {
            if (diamondFileUpload == null)
                return 0;

            try
            {
                if (diamondFileUpload.Id > 0)
                {
                    var existingRecord = await _context.DiamondFileUploadHistory
                                                       .FirstOrDefaultAsync(x => x.Id == diamondFileUpload.Id);

                    if (existingRecord != null)
                    {
                        existingRecord.NoOfSuccess = diamondFileUpload.NoOfSuccess;
                        existingRecord.NoOfFailed = diamondFileUpload.NoOfFailed;

                        _context.DiamondFileUploadHistory.Update(existingRecord);
                        await _context.SaveChangesAsync();

                        return existingRecord.Id;
                    }
                }

                var newHistory = new DiamondFileUploadHistory
                {
                    Title = diamondFileUpload.Title,
                    UploadedDate = diamondFileUpload.UploadedDate,
                    UploadedBy = diamondFileUpload.UploadedBy,
                    IsSuccess = diamondFileUpload.IsSuccess,
                    NoOfSuccess = diamondFileUpload.NoOfSuccess,
                    NoOfFailed = diamondFileUpload.NoOfFailed
                };

                await _context.DiamondFileUploadHistory.AddAsync(newHistory);
                await _context.SaveChangesAsync();

                return newHistory.Id;
            }
            catch (Exception)
            {
                // Ideally log the exception here
                return 0;
            }
        }


        public async Task<IEnumerable<DiamondData>> GetPendingDiamondList()
        {
            try
            {
                var diamondDT = await _context
                                    .Set<DiamondData>()
                                    .FromSqlRaw($"EXEC SP_PendingDiamonds")
                                    .ToListAsync();

                return diamondDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<bool> UpdateDiamondsStatus(string[] stoneIds, string userId, string status)
        //{
        //    try
        //    {
        //        var diamonds = new List<Diamond>();
        //        var diamondHistorys = new List<DiamondHistory>();
        //        var diamond = new Diamond();
        //        var diamondHistory = new DiamondHistory();

        //        for (int i = 0; i < stoneIds.Length; i++)
        //        {
        //            diamond = new Diamond();
        //            diamond = await _context.Diamonds.Where(x => x.StoneId == stoneIds[i].ToString()).FirstOrDefaultAsync();
        //            if (diamond != null)
        //            {
        //                diamond.UploadStatus = status;
        //                diamond.UpdatedDate = DateTime.Now;
        //                diamond.UpdatedBy = userId;
        //                if (diamond.UploadStatus == SD.Active)
        //                {
        //                    diamond.IsActivated = true;
        //                }
        //                else
        //                {
        //                    diamond.IsActivated = false;
        //                }

        //                diamonds.Add(diamond);
        //            }
        //        }
        //        _context.Diamonds.UpdateRange(diamonds);
        //        await _context.SaveChangesAsync();

        //        foreach (var item in diamonds)
        //        {
        //            diamondHistory = new DiamondHistory()
        //            {
        //                DiamondId = item.Id,
        //                Step = item.Step,
        //                TypeId = item.TypeId,
        //                Shade = item.Shade,
        //                ShapeId = item.ShapeId,
        //                StoneId = item.StoneId,
        //                Sku = item.Sku,
        //                DNA = item.DNA,
        //                Type = item.Type,
        //                LabId = item.LabId,
        //                Carat = item.Carat,
        //                ColorId = item.ColorId,
        //                Clarity = item.Clarity,
        //                CutId = item.CutId,
        //                PolishId = item.PolishId,
        //                SymmetryId = item.SymmetryId,
        //                Flo = item.Flo,
        //                RAP = item.RAP,
        //                Discount = item.Discount,
        //                Price = item.Price,
        //                Amount = item.Amount,
        //                Measurement = item.Measurement,
        //                Ratio = item.Ratio,
        //                Depth = item.Depth,
        //                Table = item.Table,
        //                LabShape = item.LabShape,
        //                RapAmount = item.RapAmount,
        //                DiamondVideoPath = item.DiamondVideoPath,
        //                Certificate = item.Certificate,
        //                CreatedBy = userId,
        //                CreatedDate = DateTime.Now,
        //                UpdatedBy = userId,
        //                UpdatedDate = DateTime.Now,
        //                UploadStatus = status,
        //                IsActivated = item.IsActivated,
        //            };
        //            diamondHistorys.Add(diamondHistory);
        //        }
        //        await _context.DiamondHistory.AddRangeAsync(diamondHistorys);
        //        await _context.SaveChangesAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public async Task<bool> UpdateDiamondsStatus(string[] stoneIds, string userId, string status)
        {
            try
            {
                // 1. Fetch all diamonds in a single query
                var diamonds = await _context.Diamonds
                    .Where(x => stoneIds.Contains(x.StoneId))
                    .ToListAsync();

                var now = DateTime.Now;

                // 2. Update diamond entities
                foreach (var diamond in diamonds)
                {
                    diamond.UploadStatus = status;
                    diamond.UpdatedDate = now;
                    diamond.UpdatedBy = userId;
                    diamond.IsActivated = (status == SD.Active);
                }

                // 3. Save updated diamonds in batch
                _context.Diamonds.UpdateRange(diamonds);
                await _context.SaveChangesAsync();

                // 4. Create history entries in memory
                var diamondHistories = diamonds.Select(diamond => new DiamondHistory
                {
                    DiamondId = diamond.Id,
                    Step = diamond.Step,
                    TypeId = diamond.TypeId,
                    Shade = diamond.Shade,
                    ShapeId = diamond.ShapeId,
                    StoneId = diamond.StoneId,
                    Sku = diamond.Sku,
                    DNA = diamond.DNA,
                    Type = diamond.Type,
                    LabId = diamond.LabId,
                    Carat = diamond.Carat,
                    ColorId = diamond.ColorId,
                    Clarity = diamond.Clarity,
                    CutId = diamond.CutId,
                    PolishId = diamond.PolishId,
                    SymmetryId = diamond.SymmetryId,
                    Flo = diamond.Flo,
                    RAP = diamond.RAP,
                    Discount = diamond.Discount,
                    Price = diamond.Price,
                    Amount = diamond.Amount,
                    Measurement = diamond.Measurement,
                    Ratio = diamond.Ratio,
                    Depth = diamond.Depth,
                    Table = diamond.Table,
                    LabShape = diamond.LabShape,
                    RapAmount = diamond.RapAmount,
                    DiamondVideoPath = diamond.DiamondVideoPath,
                    Certificate = diamond.Certificate,
                    CreatedBy = userId,
                    CreatedDate = now,
                    UpdatedBy = userId,
                    UpdatedDate = now,
                    UploadStatus = status,
                    IsActivated = diamond.IsActivated,
                    FluorId=diamond.FluorId,
                    ClarityId=diamond.ClarityId
                }).ToList();

                // 5. Insert all history entries in one go
                await _context.DiamondHistory.AddRangeAsync(diamondHistories);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (recommended)
                return false;
            }
        }

        public async Task<IEnumerable<DiamondData>> GetDiamondListByStatus(string status, bool isActive)
        {
            try
            {
                var diamondResults = await _context
                                    .Set<DiamondData>()
                                    .FromSqlRaw($"EXEC SP_SelectDiamondsByStatus {status},{isActive}")
                                    .ToListAsync();

                return diamondResults;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateDiamondsStatus(Diamond diamond)
        {
            try
            {
                var diamonds = new List<Diamond>();
                var diamondHistorys = new List<DiamondHistory>();
                var diamondHistory = new DiamondHistory();

                _context.Diamonds.Update(diamond);
                await _context.SaveChangesAsync();

                foreach (var item in diamonds)
                {
                    diamondHistory = new DiamondHistory()
                    {
                        DiamondId = item.Id,
                        Step = item.Step,
                        TypeId = item.TypeId,
                        Shade = item.Shade,
                        ShapeId = item.ShapeId,
                        StoneId = item.StoneId,
                        Sku = item.Sku,
                        DNA = item.DNA,
                        Type = item.Type,
                        LabId = item.LabId,
                        Carat = item.Carat,
                        ColorId = item.ColorId,
                        Clarity = item.Clarity,
                        CutId = item.CutId,
                        PolishId = item.PolishId,
                        SymmetryId = item.SymmetryId,
                        Flo = item.Flo,
                        RAP = item.RAP,
                        Discount = item.Discount,
                        Price = item.Price,
                        Amount = item.Amount,
                        Measurement = item.Measurement,
                        Ratio = item.Ratio,
                        Depth = item.Depth,
                        Table = item.Table,
                        LabShape = item.LabShape,
                        RapAmount = item.RapAmount,
                        DiamondVideoPath = item.DiamondVideoPath,
                        Certificate = item.Certificate,
                        CreatedBy = diamond.UpdatedBy,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = diamond.UpdatedBy,
                        UpdatedDate = DateTime.Now,
                        UploadStatus = item.UploadStatus,
                        IsActivated = item.IsActivated,
                    };
                    diamondHistorys.Add(diamondHistory);
                }
                await _context.DiamondHistory.AddRangeAsync(diamondHistorys);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddDiamondHistory(DiamondHistory diamondHistory)
        {
            try
            {
                await _context.DiamondHistory.AddAsync(diamondHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public async Task<List<DiamondFileUploadHistoryDTO>> GetDiamondFileUploadedHistoryList()
        {
            try
            {
                var result = await (from histroty in _context.DiamondFileUploadHistory
                                    join usr in _context.ApplicationUser on histroty.UploadedBy equals usr.Id
                                    select new DiamondFileUploadHistoryDTO
                                    {
                                        Id = histroty.Id,
                                        Title = histroty.Title,
                                        UploadedPersonName = usr.FirstName + " " + usr.LastName,
                                        UploadedBy = histroty.UploadedBy,
                                        NoOfFailed = histroty.NoOfFailed,
                                        NoOfSuccess = histroty.NoOfSuccess,
                                        IsSuccess = histroty.IsSuccess,
                                        UploadedDate = histroty.UploadedDate
                                    }).ToListAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<DiamondFileUploadHistoryDTO> GetDiamondFileUploadedHistoryById(int id)
        {
            try
            {
                var result = await (from histroty in _context.DiamondFileUploadHistory
                                    join usr in _context.ApplicationUser on histroty.UploadedBy equals usr.Id
                                    where histroty.Id == id
                                    select new DiamondFileUploadHistoryDTO
                                    {
                                        Id = histroty.Id,
                                        Title = histroty.Title,
                                        UploadedPersonName = usr.FirstName + " " + usr.LastName,
                                        UploadedBy = histroty.UploadedBy,
                                        NoOfFailed = histroty.NoOfFailed,
                                        NoOfSuccess = histroty.NoOfSuccess,
                                        IsSuccess = histroty.IsSuccess,
                                        UploadedDate = histroty.UploadedDate
                                    }).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Diamond>> GetDiamondListByHistoryId(int historyId)
        {
            var diamonds = await _context.Diamonds.Where(x => x.FileUploadHistoryId == historyId).ToListAsync();
            return diamonds;
        }

        public async Task InsertDiamondHistoryFromDiamondAsync(int historyId)
        {
            var parameter = new SqlParameter("@FileUploadHistoryId", historyId);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC SP_Add_DIAMOND_HISTORY @FileUploadHistoryId",
                parameter);
        }
    }
}
