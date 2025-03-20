using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using B2C_ECommerce.IServices;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json;


namespace B2C_ECommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            var customer = new CustomerLoginRegistrationDto();
            return View(customer);
        }


        [HttpPost]
        public async Task<IActionResult> LoginProcess(CustomerLoginDTO loginDT)
        {
            if (ModelState.IsValid)
            {
              var response=  await _accountService.CustomerSignInAsync(loginDT);

                if (response != null && response.IsAuthSuccessful)
                {
                    HttpContext.Response.Cookies.Append("Token", response.Token, new CookieOptions
                    {
                        HttpOnly = true,          // Makes the cookie inaccessible to JavaScript
                        Secure = true,            // Ensures the cookie is only sent over HTTPS
                        SameSite = SameSiteMode.Strict, // Helps mitigate CSRF attacks
                        Expires = DateTimeOffset.UtcNow.AddDays(7)  // Set cookie expiration
                    });
                    //HttpContext.Session.SetString("User", JsonConvert.SerializeObject(response.userDTO));


                    return Json(new
                    {
                        success = true,
                        token = response.Token,
                        user = response.userDTO
                    }); 
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = response?.ErrorMessage ?? "Invalid login attempt."
                    });
                }
            }

            return PartialView("~/Views/Account/LoginPartial.cshtml", loginDT);
        }


        [HttpPost]
        public IActionResult RegisterProcess(CustomerRegisterDTO registerDT)
        {
            if (ModelState.IsValid)
            {

            }

            return PartialView("~/Views/Account/RegisterPartial.cshtml", registerDT);
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("Token");

            TempData.Remove("Token");
            TempData.Remove("User");

            return RedirectToAction("Login", "Account");
        }
    }
}
