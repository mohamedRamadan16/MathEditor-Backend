using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Revisions.Commands
{
    public class UpdateRevisionCommand : IRequest<Revision>
    {
        public Revision Revision { get; }
        public UpdateRevisionCommand(Revision revision) => Revision = revision;
    }

    public class UpdateRevisionCommandHandler : IRequestHandler<UpdateRevisionCommand, Revision>
    {
        private readonly IRevisionRepository _repo;
        public UpdateRevisionCommandHandler(IRevisionRepository repo) => _repo = repo;
        public async Task<Revision> Handle(UpdateRevisionCommand request, CancellationToken cancellationToken)
            => await _repo.UpdateAsync(request.Revision);
    }
}
