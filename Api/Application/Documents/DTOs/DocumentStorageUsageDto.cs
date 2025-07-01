using System;

namespace Api.Application.Documents.DTOs
{
    public class DocumentStorageUsageDto
    {
        public Guid DocumentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RevisionCount { get; set; }
        public long TotalBytes { get; set; }
    }
}
