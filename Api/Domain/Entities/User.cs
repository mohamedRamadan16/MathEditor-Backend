using Microsoft.AspNetCore.Identity;

namespace Api.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? Handle { get; set; }
        public string Name { get; set; } = null!;
        public bool Disabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? EmailVerified { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? Image { get; set; }
        public string Role { get; set; } = "user";
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Revision> Revisions { get; set; } = new List<Revision>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<DocumentCoauthor> Coauthored { get; set; } = new List<DocumentCoauthor>();
    }
}
