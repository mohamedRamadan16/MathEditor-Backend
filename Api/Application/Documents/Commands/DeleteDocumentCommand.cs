using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Documents.Commands
{
    public class DeleteDocumentCommand : IRequest
    {
        public Guid Id { get; }
        public DeleteDocumentCommand(Guid id) => Id = id;
    }

    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand>
    {
        private readonly IDocumentRepository _repo;
        public DeleteDocumentCommandHandler(IDocumentRepository repo) => _repo = repo;
        public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            await _repo.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }
}
