using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlPanel.Services.IServices
{
    public interface IProductService
    {
        Task<ProductMstResponse> SaveNewProductListAsync(
            List<ProductDTO> products,
            string categoryName,
            string userId,
            int fileHistoryId);
    }

}
