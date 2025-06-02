using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class CollectionRepository
    {
        private readonly ApplicationDBContext _context;
        public CollectionRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<ProductCollections>> GetProductCollections() => await _context.ProductCollections.ToListAsync();
        
        public async Task<ProductCollections> GetProductCollectionById(int id) => await _context.ProductCollections.FirstOrDefaultAsync(x=>x.Id==id);
        
        //public async Task<List<ProductCollectionItems>> GetProductCollectionItems() => await _context.ProductCollections.ToListAsync();
        
    }
}
