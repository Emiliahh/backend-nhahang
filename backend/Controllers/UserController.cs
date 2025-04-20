using backend.DTOs.User;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUser userDto)
        {
            try
            {
                var userid = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userid))
                {
                    return Unauthorized();
                }

                if (!Guid.TryParse(userid, out Guid id))
                {
                    return BadRequest("Invalid user ID format.");
                }

                var user = await _userService.UpdateUser(userDto, id);
                if (user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [HttpPost]
        [Route("updatepassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassWord passWord)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userid = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userid))
                {
                    return Unauthorized();
                }
                if (!Guid.TryParse(userid, out Guid id))
                {
                    return BadRequest("Invalid user ID format.");
                }
                var result = await _userService.UpdatePassword(passWord, id);
                if (result == false)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
