using MathEditor.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MathEditor.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Document> Documents { get; set; }
    public DbSet<Revision> Revisions { get; set; }
    public DbSet<DocumentCoauthor> DocumentCoauthors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite Key for DocumentCoauthors
        modelBuilder.Entity<DocumentCoauthor>()
            .HasKey(dc => new { dc.DocumentId, dc.UserId });

        // Document -> Author (Cascade Delete)
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Author)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Document -> Base Document (NO Cascade Delete to avoid cycles)
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Base)
            .WithMany(d => d.Forks)
            .HasForeignKey(d => d.BaseId)
            .OnDelete(DeleteBehavior.NoAction);

        // Revision -> Document (NO Cascade Delete to avoid cycles)
        modelBuilder.Entity<Revision>()
            .HasOne(r => r.Document)
            .WithMany(d => d.Revisions)
            .HasForeignKey(r => r.DocumentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Revision -> Author (Cascade Delete)
        modelBuilder.Entity<Revision>()
            .HasOne(r => r.Author)
            .WithMany(u => u.Revisions)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // DocumentCoauthor -> Document (Cascade Delete)
        modelBuilder.Entity<DocumentCoauthor>()
            .HasOne(dc => dc.Document)
            .WithMany(d => d.Coauthors)
            .HasForeignKey(dc => dc.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        // DocumentCoauthor -> User (NO Cascade Delete to avoid multiple paths)
        modelBuilder.Entity<DocumentCoauthor>()
            .HasOne(dc => dc.User)
            .WithMany(u => u.CoauthoredDocuments)
            .HasForeignKey(dc => dc.UserId)
            .OnDelete(DeleteBehavior.NoAction); // Changed from Cascade to NoAction
    }

}
