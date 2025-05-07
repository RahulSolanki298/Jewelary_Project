using Microsoft.AspNetCore.Mvc;

namespace B2C_ECommerce.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
