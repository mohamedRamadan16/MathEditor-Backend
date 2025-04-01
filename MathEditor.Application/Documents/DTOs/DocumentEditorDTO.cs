namespace MathEditor.Application.Documents.DTOs;

public class EditorDocumentDto
{
    public Guid Id { get; set; }
    public string? Handle { get; set; }
    public string Name { get; set; } = default!;
    public Guid Head { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Published { get; set; }
    public bool Collab { get; set; }
    public bool Private { get; set; }
    public Guid? BaseId { get; set; }
    public string AuthorId { get; set; } = default!;
    public string Data { get; set; } = default!;
}