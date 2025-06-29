using MediatR;
using Api.Application.Documents.DTOs;

namespace Api.Application.Documents.Commands
{
    public class CreateDocumentCommand : IRequest<DocumentResponseDto?>
    {
        public DocumentCreateDto Dto { get; }
        public Guid UserId { get; }
        public CreateDocumentCommand(DocumentCreateDto dto, Guid userId)
        {
            Dto = dto;
            UserId = userId;
        }
    }
}
