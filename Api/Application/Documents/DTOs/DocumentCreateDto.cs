using System;
using System.Collections.Generic;

namespace Api.Application.Documents.DTOs
{
    public class DocumentCreateDto
    {
        public string? Handle { get; set; }
        public string Name { get; set; } = null!;
        public Guid? Head { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Published { get; set; }
        public bool Collab { get; set; }
        public bool Private { get; set; }
        public Guid? BaseId { get; set; }
        public List<string>? Coauthors { get; set; }
        public RevisionCreateDto? InitialRevision { get; set; }
    }

    public class RevisionCreateDto
    {
        public Guid? Id { get; set; }
        public string Data { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
