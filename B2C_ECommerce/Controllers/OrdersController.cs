using Microsoft.AspNetCore.Mvc;

namespace B2C_ECommerce.Controllers
{
    [Route("JewelFacet/Orders")]
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
