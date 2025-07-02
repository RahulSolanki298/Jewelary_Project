using ControlPanel.Services.IServices;
using DataAccess.Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlPanel.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDBContext _dbContext;

        public ProductService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

       public async Task<ProductMstResponse> SaveNewProductListAsync(List<ProductDTO> products, string categoryName, string userId, int fileHistoryId)
        {
            return await _dbContext.SaveNewProductListToDbAsync(
          products, categoryName, userId, fileHistoryId);
        }
    }

}
