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
            // Step 1: Get distinct ColorIds used in Diamonds
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
            // Step 1: Get distinct ColorIds used in Diamonds
            var diamondShapes = await _context.Diamonds
                                                .Select(x => x.ShapeId)
                                                .Distinct()
                                                .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
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
                    Description=par.Description,
                    IsActivated=par.IsActivated,
                    SymbolName=par.SymbolName
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

        public async Task<IEnumerable<DiamondPropertyDTO>> GetRatioListAsync()
        {
            var diamondRatio = await _context.Diamonds
                                                .Select(x => x.Ratio)
                                                .Distinct()
                                                .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
            var result = await _context.DiamondProperties
                .Where(par => diamondRatio.Contains(par.Id) && par.IsActivated)
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

        public async Task<IEnumerable<DiamondPropertyDTO>> GetFluorListAsync()
        {
            var diamondFluor = await _context.Diamonds
                                                .Select(x => x.FluorId)
                                                .Distinct()
                                                .ToListAsync();

            // Step 2: Get corresponding DiamondProperties where Id is in that list
            var result = await _context.DiamondProperties
                .Where(par => diamondFluor.Contains(par.Id) && par.IsActivated)
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
