using Microsoft.AspNetCore.Mvc;
using Models;

namespace ControlPanel.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AdminLoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Dummy authentication logic
                if (model.UserName == "admin" && model.Password == "12345678")
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(model);
        }

    }
}
