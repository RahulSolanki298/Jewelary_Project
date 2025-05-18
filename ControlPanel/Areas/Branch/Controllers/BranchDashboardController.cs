using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Areas.Branch.Controllers
{
    [Area("Branch")]
    public class BranchDashboardController : Controller
    {
        public BranchDashboardController() 
        { 
        
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
