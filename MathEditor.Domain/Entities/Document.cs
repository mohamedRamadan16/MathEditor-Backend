namespace MathEditor.Domain.Entities;
public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Handle { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid Head { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Published { get; set; } = false;
    public bool Collab { get; set; } = false;
    public bool Private { get; set; } = false;

    // Foreign Keys
    public string AuthorId { get; set; } = default!;
    public ApplicationUser Author { get; set; } = null!;

    public Guid? BaseId { get; set; }
    public Document? Base { get; set; }
    public ICollection<Document> Forks { get; set; } = new List<Document>();
    public ICollection<Revision> Revisions { get; set; } = new List<Revision>();
    public ICollection<DocumentCoauthor> Coauthors { get; set; } = new List<DocumentCoauthor>();
}
