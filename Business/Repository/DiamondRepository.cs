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
                    new SqlParameter("@Colors", SqlDbType.NVarChar) { Value = filters.Colors != null ? string.Join(",", filters.Colors) : (object)DBNull.Value },
                    new SqlParameter("@FromCarat", SqlDbType.NVarChar) { Value = filters.FromCarat ?? (object)DBNull.Value },
                    new SqlParameter("@ToCarat", SqlDbType.NVarChar) { Value = filters.ToCarat ?? (object)DBNull.Value },
                    new SqlParameter("@FromPrice", SqlDbType.Decimal) { Value = filters.FromPrice ?? (object)DBNull.Value },
                    new SqlParameter("@ToPrice", SqlDbType.Decimal) { Value = filters.ToPrice ?? (object)DBNull.Value },
                    new SqlParameter("@Cuts", SqlDbType.NVarChar) { Value = filters.Cuts != null ? string.Join(",", filters.Cuts) : (object)DBNull.Value },
                    new SqlParameter("@Clarities", SqlDbType.NVarChar) { Value = filters.Clarities != null ? string.Join(",", filters.Clarities) : (object)DBNull.Value },
                    new SqlParameter("@FromRatio", SqlDbType.Decimal) { Value = filters.FromRatio ?? (object)DBNull.Value },  // Should be decimal type, not NVarChar
                    new SqlParameter("@ToRatio", SqlDbType.Decimal) { Value = filters.ToRatio ?? (object)DBNull.Value },  // Should be decimal type, not NVarChar
                    new SqlParameter("@FromTable", SqlDbType.Decimal) { Value = filters.FromTable ?? (object)DBNull.Value },  // Should be decimal type, not NVarChar
                    new SqlParameter("@ToTable", SqlDbType.Decimal) { Value = filters.ToTable ?? (object)DBNull.Value },  // Should be decimal type, not NVarChar
                    new SqlParameter("@FromDepth", SqlDbType.Decimal) { Value = filters.FromDepth ?? (object)DBNull.Value },  // Should be decimal type, not NVarChar
                    new SqlParameter("@ToDepth", SqlDbType.Decimal) { Value = filters.ToDepth ?? (object)DBNull.Value },  // Should be decimal type, not NVarChar
                    new SqlParameter("@Polishes", SqlDbType.NVarChar) { Value = filters.Polish != null ? string.Join(",", filters.Polish) : (object)DBNull.Value },  // Fixed typo "Polish" -> "Polishes"
                    new SqlParameter("@Fluors", SqlDbType.NVarChar) { Value = filters.Fluor != null ? string.Join(",", filters.Fluor) : (object)DBNull.Value },  // Fixed typo "Fluor" -> "Fluors"
                    new SqlParameter("@Symmetries", SqlDbType.NVarChar) { Value = filters.Symmeties != null ? string.Join(",", filters.Symmeties) : (object)DBNull.Value },  // Fixed typo "Symmeties" -> "Symmetries"
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
