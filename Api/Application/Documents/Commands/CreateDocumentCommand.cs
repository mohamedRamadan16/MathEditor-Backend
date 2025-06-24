using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Documents.Commands
{
    public class CreateDocumentCommand : IRequest<Document>
    {
        public Document Document { get; }
        public CreateDocumentCommand(Document document) => Document = document;
    }

    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, Document>
    {
        private readonly IDocumentRepository _repo;
        public CreateDocumentCommandHandler(IDocumentRepository repo) => _repo = repo;
        public async Task<Document> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
            => await _repo.CreateAsync(request.Document);
    }
}
