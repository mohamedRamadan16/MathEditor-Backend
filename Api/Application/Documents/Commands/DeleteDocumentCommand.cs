using MediatR;

namespace Api.Application.Documents.Commands
{
    public class DeleteDocumentCommand : IRequest<DeleteDocumentResult>
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public DeleteDocumentCommand(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }

    public class DeleteDocumentResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}
