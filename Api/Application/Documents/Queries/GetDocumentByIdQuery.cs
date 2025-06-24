using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Documents.Queries
{
    public class GetDocumentByIdQuery : IRequest<Document?>
    {
        public Guid Id { get; }
        public GetDocumentByIdQuery(Guid id) => Id = id;
    }

    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Document?>
    {
        private readonly IDocumentRepository _repo;
        public GetDocumentByIdQueryHandler(IDocumentRepository repo) => _repo = repo;
        public async Task<Document?> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
            => await _repo.FindByIdAsync(request.Id);
    }
}
