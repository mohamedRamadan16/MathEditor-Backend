using MediatR;
using System;
using System.Collections.Generic;

namespace Api.Application.Documents.Queries
{
    public class GetDocumentStorageUsageQuery : IRequest<List<DocumentStorageUsageDto>>
    {
        public Guid UserId { get; set; }
        public GetDocumentStorageUsageQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class DocumentStorageUsageDto
    {
        public Guid DocumentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RevisionCount { get; set; }
        public long TotalBytes { get; set; }
    }
}
