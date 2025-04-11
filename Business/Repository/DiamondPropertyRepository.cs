using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Business.Repository
{
    public class DiamondPropertyRepository : IDiamondPropertyRepository
    {
        private readonly ApplicationDBContext _context;
        public DiamondPropertyRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(DiamondProperty entity)
        {
            try
            {
                await _context.DiamondProperties.AddAsync(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.DiamondProperties.FindAsync(id);
            if (entity != null)
            {
                _context.DiamondProperties.Remove(entity);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetAllAsync()
        {
            var result = await _context.DiamondProperties
                .GroupJoin(
                    _context.DiamondProperties,
                    dimPro => dimPro.ParentId,
                    propG => propG.Id,
                    (dimPro, dimGroup) => new { dimPro, dimGroup })
                .SelectMany(
                    x => x.dimGroup.DefaultIfEmpty(),
                    (x, propG) => new DiamondPropertyDTO
                    {
                        Id = x.dimPro.Id,
                        Name = x.dimPro.Name,
                        Description = x.dimPro.Description,
                        SymbolName = x.dimPro.SymbolName,
                        IconPath = x.dimPro.IconPath,
                        ParentId = x.dimPro.ParentId,
                        ParentProperty = propG.Name,  // Avoid null reference
                        IsActivated = x.dimPro.IsActivated
                    }).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetMetalListAsync()
        {
            var result = await (from par in _context.DiamondProperties
                                join col in _context.DiamondProperties on par.ParentId equals col.Id
                                where col.Name == SD.Color && par.IsActivated == true
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

        public async Task<CaratSizeRanges> GetCaratSizeRangeAsync()
        {
            var caratValues = await _context.Diamonds
                .Where(d => d.Carat != null)
                .Select(d => Convert.ToDecimal(d.Carat))
                .ToListAsync();

            if (!caratValues.Any())
                return new CaratSizeRanges(); // Return default or handle as needed

            return new CaratSizeRanges
            {
                MinCaratSize = caratValues.Min(),
                MaxCaratSize = caratValues.Max()
            };
        }

        public async Task<IEnumerable<DiamondShapeData>> GetShapeListAsync()
        {
            var result = await (from par in _context.DiamondProperties
                                join col in _context.DiamondProperties on par.ParentId equals col.Id
                                where col.Name == SD.Shape && par.IsActivated == true
                                select new DiamondShapeData
                                {
                                    Id = par.Id,
                                    Name = par.Name,
                                    ParentId = col.Id,
                                    DispOrder = par.DispOrder,
                                    IconPath = par.IconPath
                                }).OrderBy(x => x.DispOrder).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetCutListAsync()
        {
            var result = await (from dimPro in _context.DiamondProperties
                                join parent in _context.DiamondProperties on dimPro.ParentId equals parent.Id into parentGroup
                                from parentProp in parentGroup.DefaultIfEmpty()
                                where parentProp.Name == SD.Cut && dimPro.IsActivated
                                select new DiamondPropertyDTO
                                {
                                    Id = dimPro.Id,
                                    Name = dimPro.Name,
                                    Description = dimPro.Description,
                                    SymbolName = dimPro.SymbolName,
                                    IconPath = dimPro.IconPath,
                                    ParentId = dimPro.ParentId,
                                    DispOrder = dimPro.DispOrder,
                                    ParentProperty = parentProp.Name,  // Avoid null reference
                                    IsActivated = dimPro.IsActivated
                                }).OrderBy(x => x.DispOrder).ToListAsync();

            return result;
        }


        public async Task<IEnumerable<DiamondPropertyDTO>> GetClarityListAsync()
        {

            var result = await (from dimPro in _context.DiamondProperties
                                join parent in _context.DiamondProperties on dimPro.ParentId equals parent.Id into parentGroup
                                from parentProp in parentGroup.DefaultIfEmpty()
                                where parentProp.Name == SD.Clarity && dimPro.IsActivated
                                select new DiamondPropertyDTO
                                {
                                    Id = dimPro.Id,
                                    Name = dimPro.Name,
                                    Description = dimPro.Description,
                                    SymbolName = dimPro.SymbolName,
                                    IconPath = dimPro.IconPath,
                                    ParentId = dimPro.ParentId,
                                    DispOrder = dimPro.DispOrder,
                                    ParentProperty = parentProp.Name,  // Avoid null reference
                                    IsActivated = dimPro.IsActivated
                                }).OrderBy(x => x.DispOrder).ToListAsync();

            return result;

        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetRatioListAsync()
        {
            var result = await (from dimPro in _context.DiamondProperties
                                join parent in _context.DiamondProperties on dimPro.ParentId equals parent.Id into parentGroup
                                from parentProp in parentGroup.DefaultIfEmpty()
                                where parentProp.Name == SD.Ratio && dimPro.IsActivated
                                select new DiamondPropertyDTO
                                {
                                    Id = dimPro.Id,
                                    Name = dimPro.Name,
                                    Description = dimPro.Description,
                                    SymbolName = dimPro.SymbolName,
                                    IconPath = dimPro.IconPath,
                                    ParentId = dimPro.ParentId,
                                    DispOrder = dimPro.DispOrder,
                                    ParentProperty = parentProp.Name,  // Avoid null reference
                                    IsActivated = dimPro.IsActivated
                                }).OrderBy(x => x.DispOrder).ToListAsync();

            return result;
            
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetFluorListAsync()
        {
            var result = await (
                from child in _context.DiamondProperties
                join parent in _context.DiamondProperties on child.ParentId equals parent.Id
                where parent.Name == SD.Fluor && child.IsActivated
                orderby child.DispOrder
                select new DiamondPropertyDTO
                {
                    Id = child.Id,
                    Name = child.Name,
                    Description = child.Description,
                    SymbolName = child.SymbolName,
                    IconPath = child.IconPath,
                    ParentId = child.ParentId,
                    ParentProperty = parent.Name,
                    IsActivated = child.IsActivated
                }
            ).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetPolishListAsync()
        {
            var result = await (from dimPro in _context.DiamondProperties
                                join parent in _context.DiamondProperties on dimPro.ParentId equals parent.Id into parentGroup
                                from parentProp in parentGroup.DefaultIfEmpty()
                                where parentProp.Name == SD.Polish && dimPro.IsActivated
                                select new DiamondPropertyDTO
                                {
                                    Id = dimPro.Id,
                                    Name = dimPro.Name,
                                    Description = dimPro.Description,
                                    SymbolName = dimPro.SymbolName,
                                    IconPath = dimPro.IconPath,
                                    ParentId = dimPro.ParentId,
                                    DispOrder = dimPro.DispOrder,
                                    ParentProperty = parentProp.Name,  // Avoid null reference
                                    IsActivated = dimPro.IsActivated
                                }).OrderBy(x => x.DispOrder).ToListAsync();

            return result;

        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetSymmetryListAsync()
        {
            var result = await (from dimPro in _context.DiamondProperties
                                join parent in _context.DiamondProperties on dimPro.ParentId equals parent.Id into parentGroup
                                from parentProp in parentGroup.DefaultIfEmpty()
                                where parentProp.Name == SD.Symmetry && dimPro.IsActivated
                                select new DiamondPropertyDTO
                                {
                                    Id = dimPro.Id,
                                    Name = dimPro.Name,
                                    Description = dimPro.Description,
                                    SymbolName = dimPro.SymbolName,
                                    IconPath = dimPro.IconPath,
                                    ParentId = dimPro.ParentId,
                                    DispOrder = dimPro.DispOrder,
                                    ParentProperty = parentProp.Name,  // Avoid null reference
                                    IsActivated = dimPro.IsActivated
                                }).OrderBy(x => x.DispOrder).ToListAsync();

            return result;

        }

        public async Task<DiamondPropertyDTO> GetByIdAsync(int id)
        {
            var result = await _context.DiamondProperties
                 .GroupJoin(
                     _context.DiamondProperties,
                     dimPro => dimPro.ParentId,
                     propG => propG.Id,
                     (dimPro, dimGroup) => new { dimPro, dimGroup })
                 .SelectMany(
                     x => x.dimGroup.DefaultIfEmpty(),
                     (x, propG) => new DiamondPropertyDTO
                     {
                         Id = x.dimPro.Id,
                         Name = x.dimPro.Name,
                         Description = x.dimPro.Description,
                         SymbolName = x.dimPro.SymbolName,
                         IconPath = x.dimPro.IconPath,
                         ParentId = x.dimPro.ParentId,
                         ParentProperty = propG.Name,  // Avoid null reference
                         IsActivated = x.dimPro.IsActivated
                     }).FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }

        public async Task<bool> UpdateAsync(DiamondProperty entity)
        {
            try
            {
                _context.DiamondProperties.Update(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception)
            {

                return false;
            }
        }

        public async Task<int> GetDiamondPropertyId(string diamondPropertyName, string properyName)
        {
            var diamondPrt = new DiamondProperty();
            if (!string.IsNullOrEmpty(diamondPropertyName))
            {
                var parentId = await GetParentIdByName(properyName);
                diamondPrt = await _context.DiamondProperties.Where(x => x.Name == diamondPropertyName && x.ParentId == parentId).FirstOrDefaultAsync();

                if (diamondPrt == null)
                {
                    diamondPrt = new DiamondProperty()
                    {
                        Name = diamondPropertyName,
                        ParentId = parentId,
                        IsActivated = true
                    };
                    await _context.DiamondProperties.AddAsync(diamondPrt);
                    await _context.SaveChangesAsync();
                }
                return diamondPrt.Id;
            }

            return 0;

        }

        public async Task<int> GetParentIdByName(string parentName)
        {
            if (!string.IsNullOrEmpty(parentName))
            {
                var parentDT = new DiamondProperty();
                parentDT = await _context.DiamondProperties.Where(x => x.Name == parentName && string.IsNullOrEmpty(x.ParentId.ToString())).FirstOrDefaultAsync();

                if (parentDT == null)
                {
                    parentDT = new DiamondProperty()
                    {
                        Name = parentName,
                        IsActivated = true
                    };
                    await _context.DiamondProperties.AddAsync(parentDT);
                    await _context.SaveChangesAsync();
                }

                return parentDT.Id;
            }
            return 0;
        }

        public async Task<PriceRanges> GetPriceRangeAsync()
        {
            var data = await _context.Diamonds.ToListAsync();
            var priceRange = new PriceRanges();
            priceRange.MaxPrice = data.Max(x => x.Amount).Value;
            priceRange.MinPrice = data.Min(x => x.Amount).Value;

            return priceRange;
        }

        public async Task<DepthDTO> GetDepthRangeAsync()
        {
            var data = await _context.Diamonds.ToListAsync();
            var depthRange = new DepthDTO();
            depthRange.MaxValue = data.Max(x => x.Depth).Value;
            depthRange.MinValue = data.Min(x => x.Amount).Value;
            return depthRange;
        }

        public async Task<TableRangeDTO> GetTableRangeAsync()
        {
            var data = await _context.Diamonds.ToListAsync();
            var tableRange = new TableRangeDTO();
            tableRange.MaxValue = data.Max(x => x.Table).Value;
            tableRange.MinValue = data.Min(x => x.Table).Value;

            return tableRange;
        }
    }
}
