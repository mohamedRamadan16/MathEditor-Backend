using FluentValidation;
using Api.Application.Revisions.DTOs;

namespace Api.Application.Revisions.Validators
{
    public class CreateRevisionDtoValidator : AbstractValidator<CreateRevisionDto>
    {
        public CreateRevisionDtoValidator()
        {
            RuleFor(x => x.DocumentId)
                .NotEmpty().WithMessage("DocumentId is required.");

            RuleFor(x => x.Data)
                .NotEmpty().WithMessage("Data is required.")
                .MinimumLength(1).WithMessage("Data cannot be empty.");
        }
    }
}
