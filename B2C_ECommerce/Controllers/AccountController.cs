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
                        HttpOnly = true,          
                        Secure = true,            
                        SameSite = SameSiteMode.Strict, 
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Status"] = "Error";
                    TempData["Message"] = "Invalid username or password...";

                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> RegisterProcess(CustomerRegisterDTO registerDT)
        {
            if (ModelState.IsValid)
            {

                var data = new UserRequestDTO()
                {
                    Email=registerDT.EmailId,
                    FirstName=registerDT.FirstName,
                    LastName=registerDT.LastName,
                    PhoneNo=registerDT.PhoneNumber,
                    Password=registerDT.TextPassword,
                    ConfirmPassword=registerDT.ConfirmPassword
                };

                var response = await _accountService.CustomerSignUpAsync(data);

                TempData["Status"] = "Success";
                TempData["Message"] = "your application has been created successfully.";

                return RedirectToAction("Index");
            }

            return Json("Index");
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
