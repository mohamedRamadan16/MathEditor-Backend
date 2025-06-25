using MediatR;
using System;

namespace Api.Application.Documents.Commands
{
    public class UpdateDocumentHeadCommand : IRequest<bool>
    {
        public Guid DocumentId { get; }
        public Guid NewHeadId { get; }
        public UpdateDocumentHeadCommand(Guid documentId, Guid newHeadId)
        {
            DocumentId = documentId;
            NewHeadId = newHeadId;
        }
    }
}
