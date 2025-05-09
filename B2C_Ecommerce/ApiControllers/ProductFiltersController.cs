using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace B2C_ECommerce.ApiControllers
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
            var result = await _productPropertyRepository.GetSubCategoryList();
            return Ok(result);
        }

        [HttpGet("Get-Collection-List")]
        public async Task<IActionResult> GetCollectionList()
        {
            var result = await _productPropertyRepository.GetCategories();
            return Ok(result);
        }

        [HttpGet("Get-Category-List")]
        public async Task<IActionResult> GetCategoryList()
        {
            var result = await _productPropertyRepository.GetProductCategoryList();
            return Ok(result);
        }

        [HttpGet("Get-SubCat-List")]
        public async Task<IActionResult> GetSubCategoryList()
        {
            var result = await _productPropertyRepository.GetProductSubCategoryList();
            return Ok(result);
        }

        [HttpGet("get-shape-list")]
        public async Task<ActionResult> GetShapeList()
        {
            var result = await _productPropertyRepository.GetProductShapeList();
            return Ok(result);
        }

        [HttpGet("get-price-ranges")]
        public async Task<ActionResult> GetPriceRangesAsync()
        {
            var result = await _productPropertyRepository.GetPriceRangeAsync();
            return Ok(result);
        }


        [HttpGet("get-carat-ranges")]
        public async Task<ActionResult> GetCaratRangesAsync()
        {
            var result = await _productPropertyRepository.GetProductCaratSizeList();
            return Ok(result);
        }
    }
}
