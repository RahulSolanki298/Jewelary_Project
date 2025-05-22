using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
    public class SuppliersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
