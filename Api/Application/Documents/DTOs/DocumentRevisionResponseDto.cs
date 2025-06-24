using System;

namespace Api.Application.Documents.DTOs
{
    public class DocumentRevisionResponseDto
    {
        public Guid Id { get; set; }
        public string Data { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid AuthorId { get; set; }
        public DocumentUserResponseDto? Author { get; set; }
    }
}
