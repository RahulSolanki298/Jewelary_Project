using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
