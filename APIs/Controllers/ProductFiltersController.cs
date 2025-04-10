using System.Threading.Tasks;
using Business.Repository;
using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductFiltersController : ControllerBase
    {
        private readonly IProductPropertyRepository _productPropertyRepository;
        public ProductFiltersController(IProductPropertyRepository productProperty)
        {
            _productPropertyRepository = productProperty;
        }

        [HttpGet("get-color-list")]
        public async Task<ActionResult> GetColorList()
        {
            var result = await _productPropertyRepository.GetProductColorList();
            return Ok(result);
        }

        [HttpGet("Get-Style-List")]
        public async Task<IActionResult> GetStylesList()
        {
            var result = await _productPropertyRepository.GetCategories();
            return Ok(result);
        }

        [HttpGet("Get-Collection-List")]
        public async Task<IActionResult> GetCollectionList()
        {
            var result = await _productPropertyRepository.GetSubCategoryList();
            return Ok(result);
        }
    }
}
