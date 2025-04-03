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
        public DiamondRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiamondData>> GetDiamondsAsync(DiamondFilters filters, int pageNumber, int pageSize)
        {
            try
            {
                var diamonds = await _context.DiamondData
                    .FromSqlRaw("EXEC SP_GetDiamondDataBY_DiamondFilters")
                    .ToListAsync();

                var query = diamonds.ToList();

                if (filters.Colors != null && filters.Colors.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Colors.Contains(d.ColorId)).ToList();
                }

                if (filters.Carats != null && filters.Carats.Any(c=>c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Carats.Contains(d.CaratSizeId)).ToList();
                }

                if (filters.Shapes != null && filters.Shapes.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Shapes.Contains(d.ShapeId)).ToList();
                }

                if (filters.Clarities != null && filters.Clarities.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Clarities.Contains(d.ClarityId)).ToList();
                }

                if (filters.Prices != null && filters.Prices.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Prices.Contains(d.PriceNameId)).ToList();
                }

                if (filters.Ratios != null && filters.Ratios.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Ratios.Contains(d.RatioId)).ToList();
                }

                if (filters.Tables != null && filters.Tables.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Tables.Contains(d.TableId)).ToList();
                }

                if (filters.Depthes != null && filters.Depthes.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Depthes.Contains(d.DepthId)).ToList();
                }

                if (filters.Polishes != null && filters.Polishes.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Polishes.Contains(d.PolishId)).ToList();
                }

                if (filters.Fluors != null && filters.Fluors.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Fluors.Contains(d.FluorId)).ToList();
                }

                if (filters.Symmetries != null && filters.Symmetries.Any(c => c.HasValue && c.Value != 0))
                {
                    query = query.Where(d => filters.Symmetries.Contains(d.SymmetryId)).ToList();
                }

 
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var diamondList = query.ToList();

                return diamondList;
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
