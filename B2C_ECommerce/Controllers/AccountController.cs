using B2C_ECommerce.IServices;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [HttpGet]
        public IActionResult LoginProcess()
        {
            try
            {
                var customer = new CustomerLoginDTO();
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in LoginProcess GET: {Message}", ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginProcess(CustomerLoginDTO loginDTO, string returnUrl = null)
        {
            // If the user is already authenticated, skip login
            if (User.Identity.IsAuthenticated)
            {
                // Optionally: refresh cookie expiration or redirect
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
            }

            if (!ModelState.IsValid)
                return View(loginDTO);

            var response = await _accountService.CustomerSignInAsync(loginDTO);

            if (response == null || !response.IsAuthSuccessful || response.userDTO == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(loginDTO);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.userDTO.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{response.userDTO.FirstName} {response.userDTO.LastName}"),
                new Claim(ClaimTypes.Email, response.userDTO.Email ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = loginDTO.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
        }



        [HttpGet]
        public async Task<IActionResult> RegisterProcess()
        {
            var response = new CustomerRegisterDTO();
            return View(response);
        }


        [HttpPost]
        public async Task<IActionResult> RegisterProcess(CustomerRegisterDTO registerDT)
        {
            if (ModelState.IsValid)
            {
                //var captchaResponse = Request.Form["g-recaptcha-response"];
                //var secretKey = "6LdSim0rAAAAACrck7la2MF-t4OtK92wK3J9wkK5";

                //using var client = new HttpClient();
                //var postData = new Dictionary<string, string>
                //{
                //    { "secret", secretKey },
                //    { "response", captchaResponse }
                //};

                //var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(postData));
                //var responseContent = await response.Content.ReadAsStringAsync();

                //var captchaResult = JsonSerializer.Deserialize<GoogleCaptchaResponse>(responseContent);

                //if (captchaResult?.Success != true)
                //{
                //    ModelState.AddModelError("", "Captcha verification failed.");
                //    return View(registerDT);
                //}
                try
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

                    var result = await _accountService.CustomerSignUpAsync(data);

                    if (result != null)
                    {
                        TempData["Status"] = "Success";
                        TempData["Message"] = $"{data.FirstName} {data.LastName}, You're all set! Registration was successful. Welcome aboard!";
                        
                        var custData = new CustomerLoginDTO();
                        custData.Username = data.Email;
                        custData.Password = data.Password;
                        await LoginProcess(custData);
                    }

                    
                }
                catch (InvalidOperationException ex)
                {
                    // Email already exists
                    TempData["Status"] = "Error";
                    TempData["Message"] = $"Oops! Something went wrong during registration. Exception : {ex.Message} Please review the form and try again.";
                }
                catch (Exception ex)
                {
                    TempData["Status"] = "Error";
                    TempData["Message"] = "Registration failed: " + ex.Message;
                }
            }

            return View(registerDT);
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
