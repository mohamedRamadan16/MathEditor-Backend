using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Api.Application.Common.Interfaces;
using Api.Domain.Entities; // Assuming the Revision entity is in this namespace

namespace Api.Application.Revisions.Commands
{
    public class DeleteRevisionCommand : IRequest<Revision?>
    {
        public Guid Id { get; }
        public DeleteRevisionCommand(Guid id) => Id = id;
    }

    public class DeleteRevisionCommandHandler : IRequestHandler<DeleteRevisionCommand, Revision?>
    {
        private readonly IRevisionRepository _repo;
        public DeleteRevisionCommandHandler(IRevisionRepository repo) => _repo = repo;
        public async Task<Revision?> Handle(DeleteRevisionCommand request, CancellationToken cancellationToken)
        {
            var revision = await _repo.FindByIdAsync(request.Id);
            if (revision == null) return null;
            await _repo.DeleteAsync(request.Id);
            return revision;
        }
    }
}
