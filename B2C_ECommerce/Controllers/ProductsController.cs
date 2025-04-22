using System;
using System.Linq;
using System.Threading.Tasks;
using B2C_ECommerce.IServices;
using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace B2C_ECommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productRepository;

        public ProductsController(IProductService productService)
        {
            _productRepository = productService;
        }
        [HttpGet]
        public IActionResult Index(string type)
        {
            return View();
        }
            
        [HttpPost]
        public async Task<IActionResult> GetProductList(ProductFilters filters, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productRepository.GetProductListByFilter(filters,pageNumber,pageSize);
            return PartialView("~/Views/Products/_NewImagesAndProducts.cshtml", result);

        }

        [HttpGet]
        public async Task<IActionResult> GetProductFilters()
        {
            var productFilters = new ProductPropertyListDTO();
            productFilters.Colors = (await _productRepository.GetProductColorList());
            //productFilters.CollectionList = (await _productRepository.GetSubcategoryList());
            productFilters.Shapes = (await _productRepository.GetShapeList());
            //productFilters.StylesList = await _productRepository.GetCategoriesList();
            var priceDT = await _productRepository.GetProductPriceRangeData();
            productFilters.FromPrice = priceDT.MinPrice;
            productFilters.ToPrice = priceDT.MaxPrice;
            
            return PartialView("~/Views/Products/_ProductFilterBar.cshtml", productFilters);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetails(string id)
        {
            var result = await _productRepository.GetProductByProductId(id);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetailsByColorId(string sku, int? colorId=0,int? caratId=0)
        {
            var jsonResult = await _productRepository.GetProductsByColorId(sku, colorId,caratId);

            //var products = JsonConvert.DeserializeObject<ProductDTO>(jsonResult);

            return Json(jsonResult);
        }
    }
}
