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
                    }).Where(x => x.ParentProperty == $"{SD.Metal}").ToListAsync();

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
                    }).Where(x => x.ParentProperty == $"{SD.Carat}").ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DiamondPropertyDTO>> GetShapeListAsync()
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
                    }).Where(x => x.ParentProperty == $"{SD.Shape}").ToListAsync();

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
                    }).Where(x => x.ParentProperty == $"{SD.Clarity}").ToListAsync();

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
    }
}
