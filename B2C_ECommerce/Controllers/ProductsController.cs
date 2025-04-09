using System.Threading.Tasks;
using B2C_ECommerce.IServices;
using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

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
            //return PartialView("~/Views/Products/_NewImagesAndProducts.cshtml", result);
            return PartialView("~/Views/ProductNew/_NewImagesAndProducts.cshtml", result);
        }

        [HttpGet]
        public IActionResult ProductDetails(string id)
        {
            return View();
        }
    }
}
