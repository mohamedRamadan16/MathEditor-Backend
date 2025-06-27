using MediatR;
using Api.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Application.Documents.Commands
{
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, DeleteDocumentResult>
    {
        private readonly IDocumentRepository _repo;
        public DeleteDocumentCommandHandler(IDocumentRepository repo)
        {
            _repo = repo;
        }
        public async Task<DeleteDocumentResult> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var doc = await _repo.FindByIdAsync(request.Id);
            if (doc == null)
                return new DeleteDocumentResult { Success = false, Error = "Document not found" };
            if (doc.AuthorId != request.UserId)
                return new DeleteDocumentResult { Success = false, Error = "Forbidden" };
            await _repo.DeleteAsync(request.Id);
            return new DeleteDocumentResult { Success = true };
        }
    }
}
