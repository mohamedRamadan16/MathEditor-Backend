using FluentValidation;
using Api.Application.Documents.DTOs;

namespace Api.Application.Documents.Validators
{
    public class DocumentUpdateDtoValidator : AbstractValidator<DocumentUpdateDto>
    {
        public DocumentUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Document Id is required.");

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
