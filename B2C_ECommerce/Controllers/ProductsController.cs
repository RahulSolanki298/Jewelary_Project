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

        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            var result = await _productRepository.GetProductListByFilter();
            return PartialView("~/Views/ProductNew/_NewImagesAndProducts.cshtml", result);

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

            return PartialView("~/Views/Products/_ProductSideBar.cshtml", productFilters);
        }

        [HttpGet]
        public IActionResult ProductDetails(string id)
        {
            return View();
        }
    }
}
