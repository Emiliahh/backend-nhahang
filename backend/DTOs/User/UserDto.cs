using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.User
{
    public class UserDto
    {
        public string fullname { get; set; } = null!;
        public string email { get; set; } = null!;
        public string? password { get; set; }
        public string phone { get; set; } = null!;
        public string? address { get; set; }
    }

    public class UpdateUser
    {
        [MaxLength(50)]
        [MinLength(3)]
        public string? Fullname { get; set; }
        [MinLength(10)]
        [MaxLength(12)]
        public string? Phone { get; set; }
        public string? Address { get; set; }

        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
    ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ in hoa, 1 số và 1 ký tự đặc biệt")]
        public string? Password { get; set; } = null!;
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
    ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ in hoa, 1 số và 1 ký tự đặc biệt")]
        public string? OldPassword { get; set; } = null!;
    }
    public class UpdatePassWord
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ in hoa, 1 số và 1 ký tự đặc biệt")]
        public string OldPassword { get; set; } = null!;
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = null!;
    }
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.fullname)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits long.");

            RuleFor(x => x.password)
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[!@#$%^&*(),.?\":{}|<>]").WithMessage("Password must contain at least one special character.")
                .When(x => !string.IsNullOrEmpty(x.password));
        }
    }

}
