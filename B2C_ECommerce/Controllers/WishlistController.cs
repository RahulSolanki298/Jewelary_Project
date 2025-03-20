using Microsoft.AspNetCore.Mvc;

namespace B2C_ECommerce.Controllers
{
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
