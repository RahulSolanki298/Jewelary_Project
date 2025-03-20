using System;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using Common;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Helper;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class B2CController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly APISettings _aPISettings;
        private readonly IAccountRepository _accountRepository;

        public B2CController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IAccountRepository accountRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _accountRepository = accountRepository;
        }

        [HttpPost("Customer-List")]
        [AllowAnonymous]
        public async Task<ActionResult> GetB2CCustomerListAsync(bool isActived = true)
        {
            try
            {
                var result = await _userManager.GetUsersInRoleAsync("Customer");

                if (!result.Any())
                {
                    return NotFound("customer list not found");
                }

                var filteredList = result.Where(x =>
                    isActived ? x.ActivationStatus == SD.Activated : x.ActivationStatus == SD.DeActived
                ).ToList();

                return Ok(filteredList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
