using Business.Repository.IRepository;
using ControlPanel.Services.IServices;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountRepository _accountRepository;
        private readonly IOTPService _otpService;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAccountRepository accountRepository,
            IOTPService oTPService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _accountRepository = accountRepository;
            _otpService = oTPService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var loginDt = new AdminLoginModel();
            return View(loginDt);
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var roles = await _userManager.GetRolesAsync(user);

                // Store user data in cookies (you may want to encrypt or secure this in production)
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // use true in production with HTTPS
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                };

                Response.Cookies.Append("UserId", user.Id, options);
                Response.Cookies.Append("UserRoles", string.Join(",", roles), options);


                // Generate OTP
                //var otp = new Random().Next(100000, 999999).ToString();
                //HttpContext.Session.SetString("OTP", otp);
                //HttpContext.Session.SetString("UserId", user.Id);

                //// Send OTP to email and mobile (service below)
                //await _otpService.SendOtpEmailAsync(user.Email, otp);
                //await _otpService.SendOtpSmsAsync(user.PhoneNumber, otp);

                //return RedirectToAction("VerifyOtp");

                TempData["success"] = "Login successfully";

                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "Home");

                if (roles.Contains("Supplier"))
                    return RedirectToAction("Index", "SupplierDashboard");

                if (roles.Contains("Employee"))
                    return RedirectToAction("Index", "EmployeeDashboard");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }


        [HttpGet]
        public IActionResult VerifyOtp()
        {
            var dt = new VerifyOtpModel();
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpModel data)
        {
            var sessionOtp = HttpContext.Session.GetString("OTP");
            var userId = HttpContext.Session.GetString("UserId");

            if (data.OtpCode == sessionOtp)
            {
                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "Home");

                if (roles.Contains("Supplier"))
                    return RedirectToAction("Index", "SupplierDashboard");

                if (roles.Contains("Employee"))
                    return RedirectToAction("Index", "EmployeeDashboard");

                return RedirectToAction("Index", "Home"); // fallback
            }

            ModelState.AddModelError("", "Invalid OTP.");
            return View(data);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear(); // Clear all session data
            return RedirectToAction("Login", "Account");
        }

    }
}
