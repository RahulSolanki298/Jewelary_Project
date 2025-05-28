using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    public class ProductCollectionsController : Controller
    {
        public ProductCollectionsController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
