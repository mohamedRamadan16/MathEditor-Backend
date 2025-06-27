using MediatR;
using Api.Application.Documents.DTOs;
using System;

namespace Api.Application.Documents.Commands
{
    public class ForkDocumentCommand : IRequest<DocumentResponseDto?>
    {
        public Guid BaseDocumentId { get; }
        public Guid UserId { get; }
        public ForkDocumentCommand(Guid baseDocumentId, Guid userId)
        {
            BaseDocumentId = baseDocumentId;
            UserId = userId;
        }
    }
}
