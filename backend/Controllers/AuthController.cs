using backend.DTOs.User;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config = config;
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto, Response);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(new { token });
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var user = await _authService.RegisterAsync(userDto);
            if (user == null)
            {
                return BadRequest();
            }
            return Ok(user);
        }
        [Authorize]
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (user == null)
                {
                    return Unauthorized();
                }
                await _authService.LogoutAsync(user);
                Response.Cookies.Delete("auth_token");
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("authorize")]
        public async Task<IActionResult> Authorozie()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (user == null)
                {
                    return Unauthorized();
                }
                var userInfo = await _authService.GetUserInfo(user);
                return Ok(userInfo);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("promote")]
        public async Task<IActionResult> Promote()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (user == null)
                {
                    return Unauthorized();
                }
                //var userInfo = await _authService.Promote(user);
                return Ok("ni hao wo shi asmin");
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
        
        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> IssueToken()
        {
            try
            {
                var token = Request.Cookies["auth_token"];
                if (token == null)
                {
                    return Unauthorized();
                }
                var newToken = await _authService.IssueRefreshToken(token);
                return Ok(new {token= newToken });
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
    }
}

