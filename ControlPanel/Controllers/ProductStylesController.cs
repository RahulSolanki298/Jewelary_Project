using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    public class ProductStylesController : Controller
    {
        private readonly IProductStyleRepository _productStyles;
        public ProductStylesController(IProductStyleRepository productStyles)
        {
            _productStyles = productStyles;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _productStyles.GetProductStyles();
            return View(result);
        }

        //GetProductStyleItemsList
        [HttpGet]
        public async Task<IActionResult> GetProductStyleItems()
        {
            var result = await _productStyles.GetProductStyleItemsList();
            return PartialView("_ProductStyleItemList", result);
        }
    }
}
