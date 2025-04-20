using backend.DTOs.User;
using backend.Exceptions;
using backend.Models;
using backend.Services.Interfaces;
using FluentValidation;
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
        private readonly IAuthService _authService;
        private readonly IValidator<UserDto> _userDtoValidator;

        public AuthController( IAuthService authService, IValidator<UserDto> userDtoValidator)
        {
            _authService = authService;
            _userDtoValidator = userDtoValidator;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }
            try
            {
                var (res, token) = await _authService.LoginAsync(loginDto, Response);
                return Ok(new { token, user = res });
            }
            catch (UserNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (PasswordMismatchException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("loginMobile")]
        public async Task<IActionResult> LoginMobile([FromBody] LoginDto loginDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                var (res, token, rfToken) = await _authService.LoginAsyncMobile(loginDto);
                return Ok(new { token, user = res , rfToken });
            }
            catch (UserNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (PasswordMismatchException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var validationResult = await _userDtoValidator.ValidateAsync(userDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            try
            {
                var user = await _authService.RegisterAsync(userDto);
                return Ok(user);
            }
            catch (UserAlreadyExistException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
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
        public async Task<IActionResult> Authorize()
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
            catch (UserNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
        [Authorize]
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
                var userInfo = await _authService.Promote(user);
                return Ok("ni hao wo shi asmin");
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("promote-staff")]
        public async Task<IActionResult> PromoteStaff()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (user == null)
                {
                    return Unauthorized();
                }
                var userInfo = await _authService.PromoteEmployee(user);
                return Ok("ni hao wo shi asmin");
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        [Route("check-access")]
        public IActionResult CheckUser()
        {
            return Ok("you can access");

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("check-admin")]
        public IActionResult CheckAdmin()
        {
            return Ok("you can access");

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
                return Ok(new { token = newToken });
            }
            catch (InvalidTokenException e)
            {
                return Unauthorized(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpPost]
        [Route("refreshMobile")]
        public async Task<IActionResult> IssueTokenMobile([FromBody] string token)
        {
            try
            {
                var newToken = await _authService.IssueRefreshToken(token);
                return Ok(new { token = newToken });
            }
            catch (InvalidTokenException e)
            {
                return Unauthorized(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
    }
}

