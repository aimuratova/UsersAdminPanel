using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersAdminPanel.Models.Services;
using UsersAdminPanel.Models.ViewModels;

namespace UsersAdminPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserServiceMapper _userService;

        public AccountController(UserServiceMapper userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var result = await _userService.Login(model.Email, model.Password);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var result = await _userService.Register(model.Email, model.Password, model.ConfirmPassword);

            if (result.Success)
            {
                var token = await _userService.Login(model.Email, model.Password);
                if (!String.IsNullOrEmpty(token.Token))
                {
                    return Ok(token);
                }
                else
                {
                    return BadRequest(token.Message);
                }
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult LogOut()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string userId)
        {
            var user = await _userService.VerifyEmailById(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (user.Success)
            {
                return Ok("Email successfully verified!");
            }
            else
            {
                return BadRequest("Invalid verification token.");
            }
        }
    }
}
