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
            var data = await _productRepository.GetProductColorList();
            productFilters.Colors=data.ToList();
            //productFilters.CollectionList = (await _productRepository.GetSubcategoryList());
            var shapes = (await _productRepository.GetShapeList());
            productFilters.Shapes=shapes.ToList();
            var priceDT = await _productRepository.GetProductPriceRangeData();
            productFilters.FromPrice = priceDT.MinPrice;
            productFilters.ToPrice = priceDT.MaxPrice;
            
            return PartialView("~/Views/Products/_ProductFilterBar.cshtml", productFilters);
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectedProductCompare(string diamondIds)
        {
            

            return PartialView("~/Views/Products/_ProductFilterBar.cshtml");
        }


        [HttpGet]
        public async Task<IActionResult> ProductDetails(string id)
        {
            var result = await _productRepository.GetProductByProductId(id);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetailsByColorId(string sku, int? colorId=0)
        {
            var jsonResult = await _productRepository.GetProductsByColorId(sku, colorId);

            return Json(jsonResult);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetailsByCaratId(string sku, int? caratId = 0)
        {
            var jsonResult = await _productRepository.GetProductsByCaratId(sku, caratId);


            return Json(jsonResult);
        }
    }
}
