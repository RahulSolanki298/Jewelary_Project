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

        [HttpGet("GetColorList")]
        public async Task<ActionResult> GetColorList()
        {
            var result = await _productPropertyRepository.GetProductColorList();
            return Ok(result);
        }

        [HttpGet("GetStylesList")]
        public async Task<IActionResult> GetStylesList()
        {
            var result = await _productPropertyRepository.GetSubCategoryList();
            return Ok(result);
        }

        [HttpGet("GetCollectionList")]
        public async Task<IActionResult> GetCollectionList()
        {
            var result = await _productPropertyRepository.GetCategories();
            return Ok(result);
        }

        [HttpGet("GetCategoryList")]
        public async Task<IActionResult> GetCategoryList()
        {
            var result = await _productPropertyRepository.GetProductCategoryList();
            return Ok(result);
        }

        [HttpGet("GetSubCategoryList")]
        public async Task<IActionResult> GetSubCategoryList()
        {
            var result = await _productPropertyRepository.GetProductSubCategoryList();
            return Ok(result);
        }

        [HttpGet("GetShapeList")]
        public async Task<ActionResult> GetShapeList()
        {
            var result = await _productPropertyRepository.GetProductShapeList();
            return Ok(result);
        }

        [HttpGet("GetPriceRangesAsync")]
        public async Task<ActionResult> GetPriceRangesAsync()
        {
            var result = await _productPropertyRepository.GetPriceRangeAsync();
            return Ok(result);
        }


        [HttpGet("GetCaratRangesAsync")]
        public async Task<ActionResult> GetCaratRangesAsync()
        {
            var result = await _productPropertyRepository.GetProductCaratSizeList();
            return Ok(result);
        }
    }
}
