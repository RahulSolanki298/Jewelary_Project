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

        public async Task<IEnumerable<DiamondData>> GetDiamondsAsync(DiamondFilters filters, int pageNumber, int pageSize)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Color", SqlDbType.Int) { Value = filters.Color != null && filters.Color.Any() ? string.Join(",", filters.Color) : DBNull.Value },
                    new SqlParameter("@Carats", SqlDbType.Int) { Value = filters.Carats != null && filters.Carats.Any() ? string.Join(",", filters.Carats) : DBNull.Value },
                    new SqlParameter("@Shapes", SqlDbType.Int) { Value = filters.Shapes != null && filters.Shapes.Any() ? string.Join(",", filters.Shapes) : DBNull.Value },
                    new SqlParameter("@Clarity", SqlDbType.Int) { Value = filters.Clarity != null && filters.Clarity.Any() ? string.Join(",", filters.Clarity) : DBNull.Value },
                    new SqlParameter("@Price", SqlDbType.Int) { Value = filters.Price != null && filters.Price.Any() ? string.Join(",", filters.Price) : DBNull.Value },
                    new SqlParameter("@Ratio", SqlDbType.Int) { Value = filters.Ratio != null && filters.Ratio.Any() ? string.Join(",", filters.Ratio) : DBNull.Value },
                    new SqlParameter("@Table", SqlDbType.Int) { Value = filters.Table != null && filters.Table.Any() ? string.Join(",", filters.Table) : DBNull.Value },
                    new SqlParameter("@Depth", SqlDbType.Int) { Value = filters.Depth != null && filters.Depth.Any() ? string.Join(",", filters.Depth) : DBNull.Value },
                    new SqlParameter("@Polish", SqlDbType.Int) { Value = filters.Polish != null && filters.Polish.Any() ? string.Join(",", filters.Polish) : DBNull.Value },
                    new SqlParameter("@Fluor", SqlDbType.Int) { Value = filters.Fluor != null && filters.Fluor.Any() ? string.Join(",", filters.Fluor) : DBNull.Value },
                    new SqlParameter("@Symmetry", SqlDbType.Int) { Value = filters.Symmetry != null && filters.Symmetry.Any() ? string.Join(",", filters.Symmetry) : DBNull.Value },
                    new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber },
                    new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize }
                };

                //var diamonds = await _context.DiamondData
                //    .FromSqlRaw("EXEC SP_GetDiamondDataBY_DiamondFilters @Color, @Carats, @Shapes, @Clarity, @Price, @Ratio, @Table, @Depth, @Polish, @Fluor, @Symmetry, @PageNumber, @PageSize", parameters)
                //    .ToListAsync();

                return null;
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

    }
}
