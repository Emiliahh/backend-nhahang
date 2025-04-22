using backend.Data;
using backend.DTOs.User;
using backend.Exceptions;
using backend.Models;
using backend.Services.Interfaces;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Services.Implement
{
    public class UserService(NhahangContext context, UserManager<User> userManager, RoleManager<Role> roleManager) : IUserService
    {
        private readonly NhahangContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<Role> _roleManager = roleManager;

        // crud for user 
        public async Task<UserResDto> UpdateUser(UpdateUser user, Guid id)
        {
            try
            {
                var existing = _userManager.Users.FirstOrDefault(x => x.Id == id);
                if (existing == null)
                {
                    throw new UserNotFoundException("user not found");
                }
                if (user.Address != null)
                {
                    existing.Address = user.Address;
                }
                if (user.Phone != null)
                {
                    existing.Phone = user.Phone;
                }
                if (user.Fullname != null)
                {
                    existing.FullName = user.Fullname;
                }
                if (user.Password != null)
                {
                    var updatePassword = _userManager.ChangePasswordAsync(existing, user.OldPassword, user.Password);
                    if (!updatePassword.Result.Succeeded)
                    {
                        throw new PasswordMismatchException("Password mismatch");
                    }
                }
                _context.Users.Update(existing);
                await _context.SaveChangesAsync();
                return new UserResDto
                {
                    fullname = existing.FullName,
                    email = existing.Email,
                    phone = existing.Phone,
                    address = existing.Address,
                    roles = [.. _userManager.GetRolesAsync(existing).Result]
                };
            }
            catch (Exception e)
            {
                throw new Exception("Error updating user: " + e.Message);

            }
        }
        public Task<bool> UpdatePassword(UpdatePassWord passWord, Guid id)
        {
            try
            {
                var existing = _userManager.Users.FirstOrDefault(x => x.Id == id);
                if (existing == null)
                {
                    throw new UserNotFoundException("user not found");
                }
                var result = _userManager.ChangePasswordAsync(existing, passWord.OldPassword, passWord.NewPassword);
                if (result.Result.Succeeded)
                {
                    return Task.FromResult(true);
                }
                else
                {
                    throw new Exception("Error updating password: " + result.Result.Errors.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error updating password: " + e.Message);
            }
        }
        public async Task<IEnumerable<UserDto>> GetUser()
        {
            var result = await _userManager.Users.Select(x=>new UserDto
            {
                fullname=x.FullName,
                phone=x.Phone
            }).ToListAsync();
            if (result == null)
            {
                throw new UserNotFoundException("user not found");
            }
            return result;
        }

    }
}
