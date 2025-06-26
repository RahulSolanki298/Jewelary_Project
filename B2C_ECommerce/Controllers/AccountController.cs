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
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
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

        [HttpGet]
        public async Task<IActionResult> RegisterProcess()
        {
            var response = new CustomerRegisterDTO();
            return View(response);
        }


        [HttpPost]
        public async Task<IActionResult> RegisterProcess([FromBody] CustomerRegisterDTO registerDT)
        {
            if (ModelState.IsValid)
            {
                var captchaResponse = Request.Form["g-recaptcha-response"];
                var secretKey = "6LdSim0rAAAAACrck7la2MF-t4OtK92wK3J9wkK5";

                using var client = new HttpClient();
                var postData = new Dictionary<string, string>
                {
                    { "secret", secretKey },
                    { "response", captchaResponse }
                };

                var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(postData));
                var responseContent = await response.Content.ReadAsStringAsync();

                var captchaResult = JsonSerializer.Deserialize<GoogleCaptchaResponse>(responseContent);

                if (captchaResult?.Success != true)
                {
                    ModelState.AddModelError("", "Captcha verification failed.");
                    return View(registerDT);
                }

                var data = new UserRequestDTO()
                {
                    Email = registerDT.EmailId,
                    FirstName = registerDT.FirstName,
                    LastName = registerDT.LastName,
                    PhoneNo = registerDT.PhoneNumber,
                    Password = registerDT.TextPassword,
                    ConfirmPassword = registerDT.ConfirmPassword
                };

                var result = await _accountService.CustomerSignUpAsync(data);

                return Json(new {
                    Status="Success",
                    Message = $"{data.FirstName} {data.LastName} has been created successfully.",
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
