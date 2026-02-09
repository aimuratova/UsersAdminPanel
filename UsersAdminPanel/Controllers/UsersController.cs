using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UsersAdminPanel.Models.Services;

namespace UsersAdminPanel.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : Controller
    {
        private readonly UserServiceMapper _userService;

        public UsersController(UserServiceMapper userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            // user is already authenticated
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserById(userId);

            return Ok(user);
        }

        [HttpGet]
        public IActionResult List()
        {
            var users = _userService.GetAllUsers().Result.ToList();
            return PartialView("_UsersPartial", users);
        }

        [HttpPost]
        public async Task<IActionResult> Block([FromBody]List<string> idList)
        {
            var result = await _userService.BlockUser(idList);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Unblock([FromBody]List<string> idList)
        {
            var result = await _userService.UnblockUser(idList);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> idList)
        {
            var result = await _userService.DeleteUser(idList);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUnconfirmed()
        {
            var result = await _userService.DeleteUnconfirmedUser();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
