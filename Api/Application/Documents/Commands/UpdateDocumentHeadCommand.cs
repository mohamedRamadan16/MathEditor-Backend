using MediatR;
using System;

namespace Api.Application.Documents.Commands
{
    public class UpdateDocumentHeadCommand : IRequest<UpdateDocumentHeadResult>
    {
        public Guid DocumentId { get; }
        public Guid NewHeadId { get; }
        public Guid UserId { get; }
        public string UserEmail { get; }
        public UpdateDocumentHeadCommand(Guid documentId, Guid newHeadId, Guid userId, string userEmail)
        {
            DocumentId = documentId;
            NewHeadId = newHeadId;
            UserId = userId;
            UserEmail = userEmail;
        }
    }

    public class UpdateDocumentHeadResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; } // 200, 400, 403, 404
        public string? Message { get; set; }
    }
}
