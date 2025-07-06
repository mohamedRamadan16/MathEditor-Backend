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
                .NotNull().WithMessage("Data is required.")
                .Must(ValidateLexicalState).WithMessage("Invalid Lexical state structure.");
        }

        private bool ValidateLexicalState(LexicalStateDto lexicalState)
        {
            if (lexicalState?.Root == null)
                return false;

            if (lexicalState.Root.Children == null)
                return false;

            // Additional validation can be added here
            return true;
        }
    }
}
