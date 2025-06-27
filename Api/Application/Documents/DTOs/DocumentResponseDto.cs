namespace Api.Application.Documents.DTOs;

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

    public static DocumentResponseDto MapFromEntity(Api.Domain.Entities.Document doc)
    {
        return new DocumentResponseDto
        {
            Id = doc.Id,
            Handle = doc.Handle,
            Name = doc.Name,
            Head = doc.Head,
            CreatedAt = doc.CreatedAt,
            UpdatedAt = doc.UpdatedAt,
            AuthorId = doc.AuthorId,
            Published = doc.Published,
            Collab = doc.Collab,
            Private = doc.Private,
            BaseId = doc.BaseId,
            Author = doc.Author == null ? null : new DocumentUserResponseDto
            {
                Id = doc.Author.Id,
                Handle = doc.Author.Handle,
                Name = doc.Author.Name,
                Email = doc.Author.Email ?? string.Empty,
                Image = doc.Author.Image
            },
            Revisions = doc.Revisions?.Select(r => new DocumentRevisionResponseDto
            {
                Id = r.Id,
                Data = r.Data,
                CreatedAt = r.CreatedAt,
                AuthorId = r.AuthorId,
                Author = r.Author == null ? null : new DocumentUserResponseDto
                {
                    Id = r.Author.Id,
                    Handle = r.Author.Handle,
                    Name = r.Author.Name,
                    Email = r.Author.Email ?? string.Empty,
                    Image = r.Author.Image
                }
            }).ToList(),
            Coauthors = doc.Coauthors?.Where(ca => ca.User != null).Select(ca => new DocumentUserResponseDto
            {
                Id = ca.User.Id,
                Handle = ca.User.Handle,
                Name = ca.User.Name,
                Email = ca.User.Email ?? string.Empty,
                Image = ca.User.Image
            }).ToList() ?? new List<DocumentUserResponseDto>()
        };
    }
}