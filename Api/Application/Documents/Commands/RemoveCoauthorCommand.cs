using MediatR;

namespace Api.Application.Documents.Commands
{
    public class RemoveCoauthorCommand : IRequest<RemoveCoauthorResult>
    {
        public Guid DocumentId { get; }
        public string Email { get; }
        public Guid UserId { get; }
        public RemoveCoauthorCommand(Guid documentId, string email, Guid userId)
        {
            DocumentId = documentId;
            Email = email;
            UserId = userId;
        }
    }

    public class RemoveCoauthorResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? Coauthor { get; set; }
    }
}
