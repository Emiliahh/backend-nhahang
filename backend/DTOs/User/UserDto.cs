using FluentValidation;

namespace backend.DTOs.User
{
    public class UserDto
    {
        public string name { get; set; } = null!;
        public string email { get; set; } = null!;
        public string ?password { get; set; } = null!;
        public string phone { get; set; } = null!;
        public string ?address { get; set; }
    }
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{9}$").WithMessage("Phone number must be 10 digits long.");

            RuleFor(x => x.password)
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[!@#$%^&*(),.?\":{}|<>]").WithMessage("Password must contain at least one special character.")
                .When(x => !string.IsNullOrEmpty(x.password));

            RuleFor(x => x.phone)
                .NotEmpty().WithMessage("Phone number is required.");

            RuleFor(x => x.address)
                .NotEmpty().WithMessage("Address is required.");
        }
    }
}
