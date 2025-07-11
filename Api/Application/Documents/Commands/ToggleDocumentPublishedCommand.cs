using MediatR;
using Api.Application.Documents.DTOs;

namespace Api.Application.Documents.Commands
{
    public class ToggleDocumentPublishedCommand : IRequest<DocumentResponseDto?>
    {
        public Guid DocumentId { get; }
        public bool Published { get; }
        public Guid UserId { get; }
        
        public ToggleDocumentPublishedCommand(Guid documentId, bool published, Guid userId)
        {
            DocumentId = documentId;
            Published = published;
            UserId = userId;
        }
    }
}
