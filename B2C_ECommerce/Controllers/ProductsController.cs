using Microsoft.AspNetCore.Mvc;

namespace B2C_ECommerce.Controllers
{
    public class ProductsController : Controller
    {
        [HttpGet]
        public IActionResult Index(string type)
        {
            return View();
        }
    }
}
