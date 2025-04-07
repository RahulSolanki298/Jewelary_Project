using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
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

        public async Task<IEnumerable<DiamondData>> GetDiamondsAsync(DiamondFilters filters, int pageNumber, int pageSize)
        {
            try
            {
                // Construct the SQL parameters
                var parameters = new[]
                {
                    new SqlParameter("@Shapes", SqlDbType.NVarChar) { Value = filters.Shapes != null ? string.Join(",", filters.Shapes) : (object)DBNull.Value },
                    new SqlParameter("@FromColor", SqlDbType.NVarChar) { Value = filters.FromColor ?? (object)DBNull.Value },
                    new SqlParameter("@ToColor", SqlDbType.NVarChar) { Value = filters.ToColor ?? (object)DBNull.Value },
                    new SqlParameter("@FromCarat", SqlDbType.NVarChar) { Value = filters.FromCarat ?? (object)DBNull.Value },
                    new SqlParameter("@ToCarat", SqlDbType.NVarChar) { Value = filters.ToCarat ?? (object)DBNull.Value },
                    new SqlParameter("@FromPrice", SqlDbType.Decimal) { Value = filters.FromPrice ?? (object)DBNull.Value },
                    new SqlParameter("@ToPrice", SqlDbType.Decimal) { Value = filters.ToPrice ?? (object)DBNull.Value },
                    new SqlParameter("@FromCut", SqlDbType.NVarChar) { Value = filters.FromCut ?? (object)DBNull.Value },
                    new SqlParameter("@ToCut", SqlDbType.NVarChar) { Value = filters.ToCut ?? (object)DBNull.Value },
                    new SqlParameter("@FromClarity", SqlDbType.NVarChar) { Value = filters.FromClarity ?? (object)DBNull.Value },
                    new SqlParameter("@ToClarity", SqlDbType.NVarChar) { Value = filters.ToClarity ?? (object)DBNull.Value },
                    new SqlParameter("@FromRatio", SqlDbType.NVarChar) { Value = filters.FromRatio ?? (object)DBNull.Value },
                    new SqlParameter("@ToRatio", SqlDbType.NVarChar) { Value = filters.ToRatio ?? (object)DBNull.Value },
                    new SqlParameter("@FromTable", SqlDbType.NVarChar) { Value = filters.FromTable ?? (object)DBNull.Value },
                    new SqlParameter("@ToTable", SqlDbType.NVarChar) { Value = filters.ToTable ?? (object)DBNull.Value },
                    new SqlParameter("@FromDepth", SqlDbType.NVarChar) { Value = filters.FromDepth ?? (object)DBNull.Value },
                    new SqlParameter("@ToDepth", SqlDbType.NVarChar) { Value = filters.ToDepth ?? (object)DBNull.Value },
                    new SqlParameter("@FromPolish", SqlDbType.NVarChar) { Value = filters.FromPolish ?? (object)DBNull.Value },
                    new SqlParameter("@ToPolish", SqlDbType.NVarChar) { Value = filters.ToPolish ?? (object)DBNull.Value },
                    new SqlParameter("@FromFluor", SqlDbType.NVarChar) { Value = filters.FromFluor ?? (object)DBNull.Value },
                    new SqlParameter("@ToFluor", SqlDbType.NVarChar) { Value = filters.ToFluor ?? (object)DBNull.Value },
                    new SqlParameter("@FromSymmety", SqlDbType.NVarChar) { Value = filters.FromSymmety ?? (object)DBNull.Value },
                    new SqlParameter("@ToSymmety", SqlDbType.NVarChar) { Value = filters.ToSymmety ?? (object)DBNull.Value }
                };

                // Execute the stored procedure with parameters and get the result
                // SP_GetDiamondDataBY_DiamondFilters
                var diamonds = await _context.DiamondData
                    .FromSqlRaw("EXEC SP_GetDiamondDataBY_NEW_DiamondFilters @Shapes, @FromColor, @ToColor, @FromCarat, @ToCarat, @FromPrice, @ToPrice, @FromCut, @ToCut, @FromClarity, @ToClarity, @FromRatio, @ToRatio, @FromTable, @ToTable, @FromDepth, @ToDepth, @FromPolish, @ToPolish, @FromFluor, @ToFluor, @FromSymmety, @ToSymmety", parameters)
                    .ToListAsync();

                // Pagination logic (skip and take)
                var query = diamonds.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                return query;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow
                // Consider logging the exception message for debugging purposes
                throw new Exception("An error occurred while fetching diamonds.", ex);
            }
        }
        
        public async Task<IEnumerable<DiamondData>> GetDiamondList()
        {
            try
            {
                var diamonds = await _context.DiamondData
                    .FromSqlRaw("EXEC SP_GetDiamondDataBY_DiamondFilters")
                    .ToListAsync();


                return diamonds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> BulkInsertDiamondsAsync(string jsonData)
        {
            try
            {
                if (jsonData == null || !jsonData.Any()) return false;

                // Create a SQL parameter
                var jsonParam = new SqlParameter("@JsonData", jsonData);

                // Call the stored procedure
                await _context.Database.ExecuteSqlRawAsync("EXEC InsertDiamondsFromJson @JsonData", jsonParam);
                return true;
            }
            catch (Exception)
            {
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
    }
}
