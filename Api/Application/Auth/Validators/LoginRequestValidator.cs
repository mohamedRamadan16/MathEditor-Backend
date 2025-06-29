using FluentValidation;
using Api.Controllers;

namespace Api.Application.Auth.Validators
{
    public class LoginRequestValidator : AbstractValidator<AuthController.LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
