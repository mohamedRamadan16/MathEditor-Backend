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

            // Only validate Name if it's provided and not empty
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));

            // Only validate Handle if it's provided and not empty
            RuleFor(x => x.Handle)
                .NotEmpty().WithMessage("Handle cannot be empty.")
                .MaximumLength(50).WithMessage("Handle cannot exceed 50 characters.")
                .Matches("^[a-zA-Z0-9-_]+$").WithMessage("Handle can only contain letters, numbers, hyphens, and underscores.")
                .When(x => !string.IsNullOrEmpty(x.Handle));
        }
    }
}
