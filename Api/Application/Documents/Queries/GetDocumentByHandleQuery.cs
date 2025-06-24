using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Documents.Queries
{
    public class GetDocumentByHandleQuery : IRequest<Document?>
    {
        public string Handle { get; }
        public GetDocumentByHandleQuery(string handle) => Handle = handle;
    }

    public class GetDocumentByHandleQueryHandler : IRequestHandler<GetDocumentByHandleQuery, Document?>
    {
        private readonly IDocumentRepository _repo;
        public GetDocumentByHandleQueryHandler(IDocumentRepository repo) => _repo = repo;
        public async Task<Document?> Handle(GetDocumentByHandleQuery request, CancellationToken cancellationToken)
            => await _repo.FindByHandleAsync(request.Handle);
    }
}
