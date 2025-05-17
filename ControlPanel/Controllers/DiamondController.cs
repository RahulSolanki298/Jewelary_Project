using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    public class DiamondController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
