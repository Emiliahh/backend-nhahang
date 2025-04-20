using backend.DTOs.User;
using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UpdatePassword(UpdatePassWord passWord, Guid id);
        Task<UserResDto> UpdateUser(UpdateUser user, Guid id);
    }
}