using Api.Application.Revisions.DTOs;

namespace Api.Application.Documents.DTOs;

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
    public CreateRevisionDto? InitialRevision { get; set; }
}
