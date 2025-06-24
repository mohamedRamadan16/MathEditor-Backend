using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Api.Domain.Entities;

namespace Api.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Revision> Revisions { get; set; }
        public DbSet<DocumentCoauthor> DocumentCoauthors { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<VerificationToken> VerificationTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure composite keys and relationships as needed
            modelBuilder.Entity<DocumentCoauthor>()
                .HasKey(dc => new { dc.DocumentId, dc.UserEmail });

            modelBuilder.Entity<DocumentCoauthor>()
                .HasOne(dc => dc.User)
                .WithMany(u => u.Coauthored)
                .HasForeignKey(dc => dc.UserEmail)
                .HasPrincipalKey(u => u.Email)
                .OnDelete(DeleteBehavior.NoAction); // Prevent multiple cascade paths

            modelBuilder.Entity<Revision>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Revisions)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent multiple cascade paths
        }
    }
}
