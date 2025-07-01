using MediatR;
using Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Api.Application.Documents.Queries
{
    public class GetDocumentStorageUsageQueryHandler : IRequestHandler<GetDocumentStorageUsageQuery, List<DocumentStorageUsageDto>>
    {
        private readonly ApplicationDbContext _db;
        public GetDocumentStorageUsageQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<DocumentStorageUsageDto>> Handle(GetDocumentStorageUsageQuery request, CancellationToken cancellationToken)
        {
            var docs = await _db.Documents
                .Where(d => d.AuthorId == request.UserId)
                .Select(d => new DocumentStorageUsageDto
                {
                    DocumentId = d.Id,
                    Name = d.Name,
                    RevisionCount = d.Revisions.Count,
                    TotalBytes = d.Revisions.Sum(r => r.Data.Length)
                })
                .ToListAsync(cancellationToken);
            return docs;
        }
    }
}
