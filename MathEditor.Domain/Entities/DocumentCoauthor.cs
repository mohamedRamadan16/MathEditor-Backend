namespace MathEditor.Domain.Entities;

public class DocumentCoauthor
{
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public string UserEmail { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
