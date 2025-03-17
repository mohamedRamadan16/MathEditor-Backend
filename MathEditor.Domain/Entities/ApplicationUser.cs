using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;

namespace MathEditor.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? Handle { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Disabled { get; set; } = false;
    public DateTime? EmailVerified { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? Image { get; set; }
    public string Role { get; set; } = "user";

    // Navigation Properties
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<DocumentCoauthor> CoauthoredDocuments { get; set; } = new List<DocumentCoauthor>();
    public ICollection<Revision> Revisions { get; set; } = new List<Revision>();
}
