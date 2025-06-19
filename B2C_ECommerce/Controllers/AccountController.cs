using B2C_ECommerce.IServices;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace B2C_ECommerce.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(IAccountService accountService, 
            ILogger<AccountController> logger, 
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            try
            {
                var customer = new CustomerLoginRegistrationDto();
                return View(customer);
            }
            catch (Exception ex)
            {

                _logger.LogError("Exception : ", ex.Message);
                return View("Error");
            }

        }


        [HttpPost]
        public async Task<IActionResult> LoginProcess([FromBody] CustomerLoginDTO loginDT)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.CustomerSignInAsync(loginDT);

                if (response != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, response.userDTO.FirstName+" "+response.userDTO.LastName),
                        new Claim(ClaimTypes.NameIdentifier, response.userDTO.Id.ToString()) // 👈 UserId here
                    };

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MyCookieAuth", principal);


                    return Json(new { 
                    Status="Success",
                    Message= "Login Successfully."
                    });
                }
                else
                {
                    return Json(new
                    {
                        Status = "Error",
                        Message = "Invalid username or password..."
                    });
                }
            }

            return Json(new
            {
                Status = "Error",
                Message = "Pleasen enter yourname and password"
            });
        }

        



        [HttpPost]
        public async Task<IActionResult> RegisterProcess([FromBody] CustomerRegisterDTO registerDT)
        {
            if (ModelState.IsValid)
            {

                var data = new UserRequestDTO()
                {
                    Email = registerDT.EmailId,
                    FirstName = registerDT.FirstName,
                    LastName = registerDT.LastName,
                    PhoneNo = registerDT.PhoneNumber,
                    Password = registerDT.TextPassword,
                    ConfirmPassword = registerDT.ConfirmPassword
                };

                var response = await _accountService.CustomerSignUpAsync(data);

                return Json(new {
                    Status="Success",
                    Message = "your application has been created successfully.",
                });
            }

            return Json("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}
