using FluentValidation;
using Api.Application.Documents.DTOs;

namespace Api.Application.Documents.Validators
{
    public class DocumentCreateDtoValidator : AbstractValidator<DocumentCreateDto>
    {
        public DocumentCreateDtoValidator()
        {
            RuleFor(x => x.Handle)
                .NotEmpty().WithMessage("Handle is required.")
                .MaximumLength(100);
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200);
            RuleFor(x => x.InitialRevision)
                .NotNull().WithMessage("InitialRevision is required.");
            RuleFor(x => x.InitialRevision.Data)
                .NotEmpty().WithMessage("Initial revision data is required.");
        }
    }
}
