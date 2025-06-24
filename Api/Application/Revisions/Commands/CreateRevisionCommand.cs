using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Revisions.Commands
{
    public class CreateRevisionCommand : IRequest<Revision>
    {
        public Revision Revision { get; }
        public CreateRevisionCommand(Revision revision) => Revision = revision;
    }

    public class CreateRevisionCommandHandler : IRequestHandler<CreateRevisionCommand, Revision>
    {
        private readonly IRevisionRepository _repo;
        public CreateRevisionCommandHandler(IRevisionRepository repo) => _repo = repo;
        public async Task<Revision> Handle(CreateRevisionCommand request, CancellationToken cancellationToken)
            => await _repo.CreateAsync(request.Revision);
    }
}
