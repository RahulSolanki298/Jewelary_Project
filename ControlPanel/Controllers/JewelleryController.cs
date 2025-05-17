using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    public class JewelleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
