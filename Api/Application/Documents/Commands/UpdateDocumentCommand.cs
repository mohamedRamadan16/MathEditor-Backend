using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Documents.Commands
{
    public class UpdateDocumentCommand : IRequest<Document>
    {
        public Document Document { get; }
        public UpdateDocumentCommand(Document document) => Document = document;
    }

    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, Document>
    {
        private readonly IDocumentRepository _repo;
        public UpdateDocumentCommandHandler(IDocumentRepository repo) => _repo = repo;
        public async Task<Document> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
            => await _repo.UpdateAsync(request.Document);
    }
}
