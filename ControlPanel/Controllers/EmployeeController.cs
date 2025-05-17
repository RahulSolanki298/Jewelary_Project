using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
