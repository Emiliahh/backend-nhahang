using backend.DTOs.User;
using backend.Models;
using System.Security.Claims;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(UserResDto res,string token)> LoginAsync(LoginDto loginDto,HttpResponse response);
        Task<User> RegisterAsync(UserDto userDto);
        Task LogoutAsync(string user);
        Task<UserResDto> GetUserInfo(string id);
        string GenerateJwt(IList<Claim> claims);
        string RefreshToken(IList<Claim> claims);
        Task<User> Promote(string id);
        Task<User> PromoteEmployee(string id);
        string ValidateRefreshToke(string token);
        Task<string> IssueRefreshToken(string token);
        Task<(UserResDto res, string token, string rfToken)> LoginAsyncMobile(LoginDto loginDto);
    }
}