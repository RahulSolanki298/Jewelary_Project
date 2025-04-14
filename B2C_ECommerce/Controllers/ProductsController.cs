using System;
using System.Linq;
using System.Threading.Tasks;
using B2C_ECommerce.IServices;
using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;

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
        public async Task<IActionResult> GetProductList(ProductFilters filters, int pageNumber = 1, int pageSize = 5000)
        {
            var result = await _productRepository.GetProductListByFilter(filters);
            return PartialView("~/Views/Products/_NewImagesAndProducts.cshtml", result);

        }

        [HttpGet]
        public async Task<IActionResult> GetProductFilters()
        {
            var productFilters = new ProductPropertyListDTO();
            productFilters.Colors = (await _productRepository.GetProductColorList());
            productFilters.CollectionList = (await _productRepository.GetSubcategoryList());

            try { 
                productFilters.StylesList = await _productRepository.GetCategoriesList();
            }
            catch(Exception ex)
            {

            }

            return PartialView("~/Views/Products/_ProductFilterBar.cshtml", productFilters);
        }

        [HttpGet]
        public IActionResult ProductDetails(string id)
        {
            return View();
        }
    }
}
