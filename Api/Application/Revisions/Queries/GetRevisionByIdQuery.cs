using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Revisions.Queries
{
    public class GetRevisionByIdQuery : IRequest<Revision?>
    {
        public Guid Id { get; }
        public GetRevisionByIdQuery(Guid id) => Id = id;
    }

    public class GetRevisionByIdQueryHandler : IRequestHandler<GetRevisionByIdQuery, Revision?>
    {
        private readonly IRevisionRepository _repo;
        public GetRevisionByIdQueryHandler(IRevisionRepository repo) => _repo = repo;
        public async Task<Revision?> Handle(GetRevisionByIdQuery request, CancellationToken cancellationToken)
            => await _repo.FindByIdAsync(request.Id);
    }
}
