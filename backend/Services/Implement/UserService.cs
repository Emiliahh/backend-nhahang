using backend.Data;
using backend.DTOs.User;
using backend.Models;
using backend.Services.Interfaces;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using System.ComponentModel.DataAnnotations;

namespace backend.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly NhahangContext _context;
        private readonly IValidator<UserDto> _validator = new UserDtoValidator();

        public UserService(NhahangContext context)
        {
            _context = context;
            
        }
        // crud for user 
        public async Task<User> UpdateUser(UserDto user, string id)
        {
            try
            {
                var existing = await _context.Users.FindAsync(id);
                if (existing == null)
                {
                    throw new System.ComponentModel.DataAnnotations.ValidationException("Product not found");
                }
                var validate = await _validator.ValidateAsync(user);
                if (!validate.IsValid)
                {
                    throw new FluentValidation.ValidationException(validate.Errors);
                }

                //update user from object
                if (user.password != null)
                {
                    var hash = Argon2.Hash(user.password);
                    user.password = hash;
                }
                _context.Entry(existing).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
                return existing;

            }
            // handle exception
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }
        }
    }
}
