using FluentValidation;
using Api.Controllers;

namespace Api.Application.Auth.Validators
{
    public class RegisterRequestValidator : AbstractValidator<AuthController.RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Handle)
                .NotEmpty().WithMessage("Handle is required.")
                .MaximumLength(50).WithMessage("Handle cannot exceed 50 characters.")
                .Matches("^[a-zA-Z0-9-_]+$").WithMessage("Handle can only contain letters, numbers, hyphens, and underscores.");
        }
    }
}
