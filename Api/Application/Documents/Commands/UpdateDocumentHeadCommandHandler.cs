using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Documents.Commands
{
    public class UpdateDocumentHeadCommandHandler : IRequestHandler<UpdateDocumentHeadCommand, UpdateDocumentHeadResult>
    {
        private readonly ApplicationDbContext _db;
        public UpdateDocumentHeadCommandHandler(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<UpdateDocumentHeadResult> Handle(UpdateDocumentHeadCommand request, CancellationToken cancellationToken)
        {
            var doc = await _db.Documents
                .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);
            if (doc == null)
                return new UpdateDocumentHeadResult { Success = false, StatusCode = 404, Message = "Document not found." };
            // Only author or coauthor can update head
            var isCoauthor = doc.Coauthors.Any(ca =>
                (ca.User != null && ca.User.Id == request.UserId) ||
                (!string.IsNullOrEmpty(request.UserEmail) && ca.UserEmail == request.UserEmail)
            );
            if (doc.AuthorId != request.UserId && !isCoauthor)
                return new UpdateDocumentHeadResult { Success = false, StatusCode = 403, Message = "Forbidden: Not document author or coauthor." };
            // Validate revision exists and belongs to this document
            var revision = await _db.Revisions.FirstOrDefaultAsync(r => r.Id == request.NewHeadId && r.DocumentId == request.DocumentId, cancellationToken);
            if (revision == null)
                return new UpdateDocumentHeadResult { Success = false, StatusCode = 400, Message = "Revision does not exist or does not belong to this document." };
            doc.Head = request.NewHeadId;
            doc.UpdatedAt = System.DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
            return new UpdateDocumentHeadResult { Success = true, StatusCode = 200, Message = "Document head updated." };
        }
    }
}
