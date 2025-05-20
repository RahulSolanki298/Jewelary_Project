using B2C_ECommerce.IServices;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Threading.Tasks;

namespace B2C_ECommerce.Controllers
{
    public class ProductProgramsController : Controller
    {
        private readonly IProductService _productRepository;

        public ProductProgramsController(IProductService productService)
        {
            _productRepository = productService;
        }

        [HttpGet]
        public IActionResult Index(string programName,string type)
        {
            ViewBag.ProgramName = programName;
            ViewBag.Type = type;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productRepository.GetProductListByFilter(filters, pageNumber, pageSize);
            return PartialView("_NewImagesAndProducts", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDataFilters()
        {
            var productFilters = new ProductPropertyListDTO();
            var data = await _productRepository.GetProductColorList();
            productFilters.Colors = data.ToList();
            //productFilters.CollectionList = (await _productRepository.GetSubcategoryList());
            var shapes = (await _productRepository.GetShapeList());
            productFilters.Shapes = shapes.ToList();
            var priceDT = await _productRepository.GetProductPriceRangeData();
            productFilters.FromPrice = priceDT.MinPrice;
            productFilters.ToPrice = priceDT.MaxPrice;

            return PartialView("_ProductFilterBar", productFilters);
        }

        [HttpGet]
        public IActionResult GetSelectedProductCompare(string diamondIds)
        {
            return PartialView("_ProductFilterBar", null);
        }


        [HttpGet]
        public async Task<IActionResult> ProductDetails(string id)
        {
            var result = await _productRepository.GetProductByProductId(id);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetailsByColorId(string sku, int? colorId = 0)
        {
            var jsonResult = await _productRepository.GetProductsByColorId(sku, colorId);


            return Json(jsonResult);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetailsByCaratId(string sku, int? caratId = 0)
        {
            var jsonResult = await _productRepository.GetProductsByColorId(sku, 0, caratId);


            return Json(jsonResult);
        }

    }
}
