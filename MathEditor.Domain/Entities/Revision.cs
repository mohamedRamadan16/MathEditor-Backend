namespace MathEditor.Domain.Entities;

public class Revision
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Data { get; set; } = string.Empty; // JSON stored as string
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public string AuthorId { get; set; } = default!;
    public ApplicationUser Author { get; set; } = null!;
}