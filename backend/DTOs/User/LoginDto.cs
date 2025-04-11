using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; } = null!;
    }
}
