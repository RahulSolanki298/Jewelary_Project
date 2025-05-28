using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    public class ProductStylesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
