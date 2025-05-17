using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    public class SuppliersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
