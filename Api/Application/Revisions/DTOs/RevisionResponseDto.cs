namespace Api.Application.Revisions.DTOs
{
    public class RevisionResponseDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuthorDto Author { get; set; } = null!;
        public string Data { get; set; } = null!;
    }

    public class AuthorDto
    {
        public Guid Id { get; set; }
        public string? Handle { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
    }
}
