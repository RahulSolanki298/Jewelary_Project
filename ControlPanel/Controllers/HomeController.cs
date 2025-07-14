using Business.Repository.IRepository;
using Common;
using ControlPanel.Models;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{

    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IB2COrdersRepository _b2COrdersRepository;

        public HomeController(ILogger<HomeController> logger,
                                UserManager<ApplicationUser> userManager,
                                IB2COrdersRepository b2COrdersRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _b2COrdersRepository = b2COrdersRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.b2cData = await _b2COrdersRepository.GetB2COrderDeliveredList();
            ViewBag.customerList = await _userManager.GetUsersInRoleAsync(SD.Customer);
            ViewBag.Revenue = await _b2COrdersRepository.GetB2COrderDeliveredList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {

            var userId = HttpContext.Request.Cookies["UserId"];

            if (string.IsNullOrEmpty(userId))
            {
                return Json("User is not logged in or cookie expired.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
