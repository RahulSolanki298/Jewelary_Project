using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Repository
{
    public class B2BOrdersRepository : IB2BOrdersRepository
    {
        private readonly ApplicationDBContext _context;
        public B2BOrdersRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Orders>> GetB2BOrderReqs()
        {
            return await _context.Orders.ToListAsync();
        }
       
    }
}
