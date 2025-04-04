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
        private readonly IB2COrdersRepository _ordersRepository;

        public B2CController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IAccountRepository accountRepository,
            IB2COrdersRepository ordersRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _accountRepository = accountRepository;
            _ordersRepository = ordersRepository;
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

        [HttpPost("Order-Request-List")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrderRequestListAsync()
        {
            var response = await _ordersRepository.GetB2COrderRequestList();
            return Ok(response);
        }

        [HttpPost("Order-Accept-List")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrderAcceptListAsync()
        {
            var response = await _ordersRepository.GetB2COrderAcceptList();
            return Ok(response);
        }

        [HttpPost("Order-Reject-List")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrderRejectListAsync()
        {
            var response = await _ordersRepository.GetB2COrderRejectList();
            return Ok(response);
        }


        [HttpPost("Order-Processing-List")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrderProcessingAsync()
        {
            var response = await _ordersRepository.GetB2COrderProcessingList();
            return Ok(response);
        }

        [HttpPost("Order-Shipped-List")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrderShippedAsync()
        {
            var response = await _ordersRepository.GetB2COrderShippedList();
            return Ok(response);
        }

        [HttpPost("Order-ReadyForShipment-List")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrderReadyForShipmentAsync()
        {
            var response = await _ordersRepository.GetB2COrderReadyForShipmentList();
            return Ok(response);
        }

        [HttpPost("Order-Complated-List")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrderComplatedAsync()
        {
            var response = await _ordersRepository.GetB2COrderComplatedList();
            return Ok(response);
        }
    }
}
