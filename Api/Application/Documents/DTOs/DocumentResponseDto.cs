using System;
using System.Collections.Generic;

namespace Api.Application.Documents.DTOs
{
    public class DocumentResponseDto
    {
        public Guid Id { get; set; }
        public string? Handle { get; set; }
        public string Name { get; set; } = null!;
        public Guid Head { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid AuthorId { get; set; }
        public bool Published { get; set; }
        public bool Collab { get; set; }
        public bool Private { get; set; }
        public Guid? BaseId { get; set; }
        public DocumentUserResponseDto? Author { get; set; }
        public List<DocumentUserResponseDto>? Coauthors { get; set; }
        public List<DocumentRevisionResponseDto>? Revisions { get; set; }
    }

    public class DocumentUserResponseDto
    {
        public Guid Id { get; set; }
        public string? Handle { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Image { get; set; }
    }
}
