using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Documents.Commands
{
    public class UpdateDocumentHeadCommandHandler : IRequestHandler<UpdateDocumentHeadCommand, bool>
    {
        private readonly ApplicationDbContext _db;
        public UpdateDocumentHeadCommandHandler(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Handle(UpdateDocumentHeadCommand request, CancellationToken cancellationToken)
        {
            var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);
            if (doc == null) return false;
            doc.Head = request.NewHeadId;
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
