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
                    }).Where(x => x.ParentProperty == $"{SD.Color}" && x.IsActivated == true).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetCaratListAsync()
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
                    }).Where(x => x.ParentProperty == $"{SD.CaratSize}" && x.IsActivated == true).ToListAsync();

            return result;
        }

        public async Task<CaratSizeRanges> GetCaratSizeRangeAsync()
        {
            var ParentId = await GetParentIdByName(SD.CaratSize);
            var caratSizes = await _context.DiamondProperties
                .Where(x => x.ParentId == ParentId && x.IsActivated == true)
                .Select(x => x.Name)  // Name represents carat size
                .ToListAsync();

            if (!caratSizes.Any())
            {
                return null;
            }

            // Convert to decimal
            var caratValues = caratSizes
                .Select(x => decimal.TryParse(x, out var val) ? val : (decimal?)null)
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToList();

            var Response = new CaratSizeRanges()
            {
                MinCaratSize = caratValues.Min(),
                MaxCaratSize = caratValues.Max()
            };

            return Response;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetShapeListAsync()
        {
            var result = await (from dimPro in _context.DiamondProperties
                                join parent in _context.DiamondProperties on dimPro.ParentId equals parent.Id into parentGroup
                                from parentProp in parentGroup.DefaultIfEmpty()
                                where parentProp.Name == SD.Shape && dimPro.IsActivated
                                select new DiamondPropertyDTO
                                {
                                    Id = dimPro.Id,
                                    Name = dimPro.Name,
                                    Description = dimPro.Description,
                                    SymbolName = dimPro.SymbolName,
                                    IconPath = dimPro.IconPath,
                                    ParentId = dimPro.ParentId,
                                    ParentProperty = parentProp.Name,  // Avoid null reference
                                    IsActivated = dimPro.IsActivated
                                }).ToListAsync();

            return result;
        }


        public async Task<IEnumerable<DiamondPropertyDTO>> GetClarityListAsync()
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
                    }).Where(x => x.ParentProperty == $"{SD.Clarity}" && x.IsActivated == true).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetRatioListAsync()
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
                    }).Where(x => x.ParentProperty == $"{SD.Clarity}" && x.IsActivated == true).ToListAsync();

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
    }
}
