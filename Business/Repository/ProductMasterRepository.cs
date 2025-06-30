using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class ProductMasterRepository /*: IProductMasterRepository*/
    {
        private readonly ApplicationDBContext _context; 
        public ProductMasterRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        //public async Task<List<ProductMasterDTO>> GetProductMasterList() 
        //{
           
        //   return await _context.ProductMaster.ToListAsync(); 
        //}

    }
}
