using backend.DTOs.User;
using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> UpdateUser(UserDto user, string id);
    }
}