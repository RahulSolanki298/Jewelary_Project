using APIs.Helper;
using Business.Repository.IRepository;
using Common;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly APISettings _aPISettings;
        private readonly IAccountRepository _accountRepository;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<APISettings> options,
            IAccountRepository accountRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _aPISettings = options.Value;
            _accountRepository = accountRepository;
        }

        [HttpPost("/ClientSignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] UserRequestDTO userRequestDTO)
        {
            if (userRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new ApplicationUser
            {
                UserName = userRequestDTO.Email,
                Email = userRequestDTO.Email,
                FirstName = userRequestDTO.FirstName,
                LastName = userRequestDTO.LastName,
                PhoneNumber = userRequestDTO.PhoneNo,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userRequestDTO.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterationResponseDTO
                { Errors = errors, IsRegisterationSuccessful = false });
            }
            var roleResult = await _userManager.AddToRoleAsync(user, SD.Customer);
            if (!roleResult.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterationResponseDTO
                { Errors = errors, IsRegisterationSuccessful = false });
            }
            return StatusCode(201);
        }


        [HttpPost("/ClientSignIn")]
        [AllowAnonymous]
        public async Task<ActionResult> SignIn([FromBody] CustomerLoginDTO authenticationDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(authenticationDTO.Username,
                authenticationDTO.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(authenticationDTO.Username);
                if (user == null)
                {
                    return Unauthorized(new AuthenticationResponseDTO
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "Invalid Authentication"
                    });
                }

                //everything is valid and we need to login the user

                var signinCredentials = GetSigningCredentials();
                var claims = await GetClaims(user);

                var tokenOptions = new JwtSecurityToken(
                    issuer: _aPISettings.ValidIssuer,
                    audience: _aPISettings.ValidAudience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials: signinCredentials);

                var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = true,
                    Token = token,
                    userDTO = new UserDTO
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Id = user.Id,
                        Email = user.Email,
                        PhoneNo = user.PhoneNumber
                    }
                });
            }
            else
            {
                return Unauthorized(new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid Authentication"
                });
            }
        }

        [HttpPost("/B2BSignUp")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateBusinessAccount([FromBody] BusinessAccountRegisterDTO buzzRequest)
        {

            if (buzzRequest == null)
            {
                return BadRequest("Business account data is required.");
            }

            try
            {
                var response = await _accountRepository.UpsertBusinessAccountAsync(buzzRequest);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logger here)
                // _logger.LogError(ex, "An error occurred while creating the business account");

                // Return a general error response with a message
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("/SupplierSignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SupplierSignUp([FromBody] UserRequestDTO userRequestDTO)
        {
            if (userRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new ApplicationUser
            {
                UserName = userRequestDTO.Email,
                Email = userRequestDTO.Email,
                FirstName = userRequestDTO.FirstName,
                LastName = userRequestDTO.LastName,
                PhoneNumber = userRequestDTO.PhoneNo,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userRequestDTO.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterationResponseDTO
                { Errors = errors, IsRegisterationSuccessful = false });
            }
            var roleResult = await _userManager.AddToRoleAsync(user, SD.Supplier);
            if (!roleResult.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterationResponseDTO
                { Errors = errors, IsRegisterationSuccessful = false });
            }
            return StatusCode(201);
        }


        private SigningCredentials GetSigningCredentials()
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_aPISettings.SecretKey));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Email),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("Id",user.Id),

            };
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(user.Email));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
