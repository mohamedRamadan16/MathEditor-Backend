using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Api.Domain.Entities
{
    public class Document
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
        public Document? Base { get; set; }
        public ICollection<Document> Forks { get; set; } = new List<Document>();
        public ICollection<Revision> Revisions { get; set; } = new List<Revision>();
        public User Author { get; set; } = null!;
        public ICollection<DocumentCoauthor> Coauthors { get; set; } = new List<DocumentCoauthor>();
    }

    public class Revision
    {
        public Guid Id { get; set; }
        public string Data { get; set; } = null!; // JSON
        public DateTime CreatedAt { get; set; }
        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
    }

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

    public class DocumentCoauthor
    {
        public Guid DocumentId { get; set; }
        public string UserEmail { get; set; } = null!;
        public Document Document { get; set; } = null!;
        public User User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public string ProviderAccountId { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
        public int? ExpiresAt { get; set; }
        public string? TokenType { get; set; }
        public string? Scope { get; set; }
        public string? IdToken { get; set; }
        public string? SessionState { get; set; }
        public string? OAuthTokenSecret { get; set; }
        public string? OAuthToken { get; set; }
        public User User { get; set; } = null!;
    }

    public class Session
    {
        public Guid Id { get; set; }
        public string SessionToken { get; set; } = null!;
        public Guid UserId { get; set; }
        public DateTime Expires { get; set; }
        public User User { get; set; } = null!;
    }

    public class VerificationToken
    {
        public int Id { get; set; }
        public string Identifier { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
}
