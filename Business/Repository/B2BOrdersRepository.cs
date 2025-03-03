using System.Collections.Generic;
using System.Linq;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;

namespace Business.Repository
{
    public class B2BOrdersRepository : IB2BOrdersRepository
    {
        private readonly ApplicationDBContext _context;
        public B2BOrdersRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<Orders> GetB2BOrderList()
        {
            return new List<Orders>();
        }
    }
}
